using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstruction : MonoBehaviour {

	public float pozObsx1;
	public float pozObsx2;
	public float pozObsz1;
	public float pozObsz2;
	public float pozObs_z1;
	public float pozObs_z2;
	public float pozObs_x1;
	public float pozObs_x2;

	public float pozObsx1_;
	public float pozObsx2_;
	public float pozObsz1_;
	public float pozObsz2_;
	public float pozObs_z1_;
	public float pozObs_z2_;
	public float pozObs_x1_;
	public float pozObs_x2_;

	public float pozObsx1_one;
	public float pozObsx2_one;
	public float pozObsz1_one;
	public float pozObsz2_one;
	public float pozObs_z1_one;
	public float pozObs_z2_one;
	public float pozObs_x1_one;
	public float pozObs_x2_one;



	
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	void OnTriggerStay (Collider col){
		if(col.gameObject.name == "pozObsx1"){
			if(Input.GetKeyDown(KeyCode.Space)){
				gameObject.transform.localPosition += new Vector3(pozObsx1,0,0);
			}
		}
		if(col.gameObject.name == "pozObsx2"){
			if(Input.GetKeyDown(KeyCode.Space)){
				gameObject.transform.localPosition += new Vector3(pozObsx2,0,0);
			}
		}
		if(col.gameObject.name == "pozObsz1"){
			if(Input.GetKeyDown(KeyCode.Space)){
				gameObject.transform.localPosition += new Vector3(0,0,pozObsz1);
			}
		}
		if(col.gameObject.name == "pozObsz2"){
			if(Input.GetKeyDown(KeyCode.Space)){
				gameObject.transform.localPosition += new Vector3(0,0,pozObsz2);
			}
		}
		if(col.gameObject.name == "pozObs_z1"){
			if(Input.GetKeyDown(KeyCode.Space)){
				for(float i = pozObs_z1_; pozObs_z1_ >= pozObs_z1; pozObs_z1_ += pozObs_z1_one){
					gameObject.transform.localPosition += new Vector3(0,0,pozObs_z1_);
				}
			}
		}
		if(col.gameObject.name == "pozObs_z2"){
			if(Input.GetKeyDown(KeyCode.Space)){
				gameObject.transform.localPosition += new Vector3(0,0,pozObs_z2);
			}
		}
		if(col.gameObject.name == "pozObs_x1"){
			if(Input.GetKeyDown(KeyCode.Space)){
				gameObject.transform.localPosition += new Vector3(pozObs_x1,0,0);
			}
		}
		if(col.gameObject.name == "pozObs_x2"){
			if(Input.GetKeyDown(KeyCode.Space)){
				gameObject.transform.localPosition += new Vector3(pozObs_x2,0,0);
			}
		}
	}
	void OnTriggerExit (Collider col){
		if(col.gameObject.name == "pozObsx1"){
			
		}
	}
}
