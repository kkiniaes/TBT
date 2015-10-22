using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {


	private GameObject detailGroup;
	private GameObject headTitle;
	private GameObject massText;
	private GameObject chargeText;
	//private GameObject quantamEntangleText;
	private GameObject selectedToolHighlight;

	private int selectedTool = 1;

	// Use this for initialization
	void Start () {
		detailGroup = transform.FindChild("Canvas").FindChild("DetailGroup").gameObject;
		headTitle = detailGroup.transform.FindChild("Head").FindChild("HeadTitleContainer").FindChild("HeadTitle").gameObject;
		massText = detailGroup.transform.FindChild("Head").FindChild("Detail1").FindChild("MassTextContainer").FindChild("MassText").gameObject;
		chargeText = detailGroup.transform.FindChild("Head").FindChild("Detail2").FindChild("ChargeTextContainer").FindChild("ChargeText").gameObject;
		//quantamEntangleText = detailGroup.transform.FindChild("Head").FindChild("Detail3").FindChild("QuantamEntangleTextContainer").FindChild("QuantamEntangleText").gameObject;
		selectedToolHighlight = transform.FindChild("Canvas").FindChild("Tools").FindChild("SelectedTool").gameObject;
	}
	
	// Update is called once per frame
	void Update () {

		//detailGroup.transform.localScale = Vector3.one*Screen.width/1024f*4f;

		if(Player.instance.LookingAtObject != null) {
			detailGroup.GetComponent<Animator>().SetBool("PaneOpen", true);
			massText.GetComponent<Text>().text = (((int)(((int)(Player.instance.LookingAtObject.GetComponent<PhysicsModifyable>().mass*1000f))/600f)*10f)/10f) + "kg";
			chargeText.GetComponent<Text>().text = (Player.instance.LookingAtObject.GetComponent<PhysicsModifyable>().charge > 0) ? "+" : (Player.instance.LookingAtObject.GetComponent<PhysicsModifyable>().charge < 0 ? "-" : "0");
			if(Player.instance.LookingAtObject.GetComponent<Goal>() != null) {
				headTitle.GetComponent<Text>().text = System.Enum.GetNames(typeof(VerboseElement))[Player.instance.LookingAtObject.GetComponent<Goal>().numElementsCombined-1];
			} else {
				if(Player.instance.LookingAtObject.GetComponent<PhysicsAffected>() != null) {
					headTitle.GetComponent<Text>().text = "Gravity Affected";
				} else {
					headTitle.GetComponent<Text>().text = "Static";
				}
				if(Player.instance.LookingAtObject.GetComponent<PhysicsModifyable>() != null) {
					if(Player.instance.LookingAtObject.GetComponent<PhysicsModifyable>().antiMatter) {
						headTitle.GetComponent<Text>().text += " Antimatter";
					}
				}
			}
		} else {
			detailGroup.GetComponent<Animator>().SetBool("PaneOpen", false);
		}

		if(Input.GetKey(KeyCode.Alpha1)) {
			selectedTool = 1;
		}
		if(Input.GetKey(KeyCode.Alpha2)) {
			selectedTool = 2;
		}
		if(Input.GetKey(KeyCode.Alpha3)) {
			selectedTool = 3;
		}

//		selectedToolHighlight.transform.localPosition = Vector3.MoveTowards(selectedToolHighlight.transform.localPosition, new Vector3((selectedTool - 2)*58f, 0f, 0f), Time.deltaTime*50f);
		selectedToolHighlight.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(selectedToolHighlight.GetComponent<RectTransform>().anchoredPosition, new Vector2((selectedTool - 2)*58f, -25f), Time.deltaTime*500f);
	}
}
