using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        float ws;
        float rs;
        float si;
        float StartTime;
        public bool Pause;
        public GameObject Me; 
        public Text textSpeed;
        public Text textWalk;
        [SerializeField] public bool m_IsWalking;
        [SerializeField] public float m_WalkSpeed;
        [SerializeField] public float m_RunSpeed;
        [SerializeField] public float m_SiteSpeed;
        [SerializeField] public float m_FastSiteSpeed;
        [SerializeField] public float m_Acceleration;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] public float m_StickToGroundForce;
        [SerializeField] public float m_GravityMultiplier;
        [SerializeField] public MouseLook m_MouseLook;
        [SerializeField] public bool m_UseFovKick;
        [SerializeField] public FOVKick m_FovKick = new FOVKick();
        [SerializeField] public bool m_UseHeadBob;
        [SerializeField] public CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] public float m_StepInterval;
        [SerializeField] public float m_SitingStepInterval;
        [SerializeField] public AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.        // the sound played when character leaves the ground.
        [SerializeField] public AudioClip m_LandSound;  
        [SerializeField] public bool m_Site;
               // the sound played when character touches back on ground.

        public Camera m_Camera;
        public float m_YRotation;
        public Vector2 m_Input;
        public Vector3 m_MoveDir = Vector3.zero;
        public CharacterController m_CharacterController;
        public CollisionFlags m_CollisionFlags;
        public bool m_PreviouslyGrounded;
        public Vector3 m_OriginalCameraPosition;
        public float m_StepCycle;
        public float m_NextStep;
        public AudioSource m_AudioSource;

        // Use this for initialization
        private void Start()
        {
            ws = m_WalkSpeed;
            rs = m_RunSpeed;
            si = m_StepInterval;
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
            
        }


        // Update is called once per frame
        public void Update()
        {
           
            if(Pause == false){
                
                RotateView();
                 if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
                {
                    PlayLandingSound();
                    m_MoveDir.y = 0f;
                }
                if (!m_CharacterController.isGrounded && m_PreviouslyGrounded)
                {
                    m_MoveDir.y = 0f;
                }

                m_PreviouslyGrounded = m_CharacterController.isGrounded;
                
                if(m_Site == false){
                    if(Input.GetKeyDown(KeyCode.LeftControl)){
                        m_Site = true;
                    }
                }else{
                    if(Input.GetKeyDown(KeyCode.LeftControl)){
                        m_Site = false;
                    }  
                }
                if(m_Site == true){
                    Site();
                }
                if(m_Site == false){
                    Up();
                }
                textWalk.text = m_WalkSpeed.ToString();
                textSpeed.text = m_RunSpeed.ToString();

            }
            
            // the jump state needs to read here to make sure it is not missed

        }
        public void Site(){
            Me.transform.localScale = new Vector3(2f,1f,2f);
            m_WalkSpeed = m_SiteSpeed;
            m_RunSpeed = m_FastSiteSpeed;
            m_StepInterval = m_SitingStepInterval;

        }
        public void Up(){
            Me.transform.localScale = new Vector3(2f,2f,2f);
            m_WalkSpeed = ws;
            m_RunSpeed = rs;
            m_StepInterval = si;
        }
        public void PlayLandingSound()
        {
            if(Pause == false){
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
            }
        }
        public void FixedUpdate()
        {
            if(Pause == false){
                float speed;
                GetInput(out speed);
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                m_MoveDir.x = desiredMove.x*speed;
                m_MoveDir.z = desiredMove.z*speed;


                if (m_CharacterController.isGrounded)
                {
                    m_MoveDir.y = -m_StickToGroundForce;
                }
                else
                {
                    m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
                }
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

                ProgressStepCycle(speed);
                UpdateCameraPosition(speed);

                m_MouseLook.UpdateCursorLock();
                
                    
            }
        }


        public void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        public void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        public void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;

            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        public void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        public void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
