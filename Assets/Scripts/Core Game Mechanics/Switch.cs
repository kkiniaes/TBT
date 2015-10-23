using UnityEngine;
using System.Collections;

// Affects a certain object when turned on or off. Triggered by lightning.
public class Switch : MonoBehaviour {

	// The object affected when the switch is triggered.
	public PhysicsModifyable attachedObject;
	// Whether the switch is on or off.
	public bool activated = false;
	// The index of the switch (used if the switch has multiple switch components).
	private int switchIndex;
	public int SwitchIndex {
		get { return switchIndex; }
	}

	static Color[] particleColors = new Color[]{
		new Color(0.047f, 0.522f, 0.914f),
		new Color(0.1f, 0, 0),
		new Color(0, 0.1f, 0),
		new Color(0.1f, 0.1f, 0),
		new Color(0.1f, 0, 0.1f)
	};

	// Particles emitted when the switch is on.
	private static GameObject switchParticles;
	// The line drawn between the switch and its attached object.
	private static GameObject switchLine;

	// Set switch indices before a state is initialized.
	void Awake () {
		switchIndex = GetComponent<PhysicsModifyable> ().SwitchCounter++;
	}

	// Use this for initialization
	protected void Start () {
		if (switchParticles == null) {
			switchParticles = Resources.Load<GameObject> ("SwitchParticles");
			switchLine = Resources.Load<GameObject> ("SwitchLine");
		}

		GameObject particleObject = Instantiate (switchParticles, transform.position, transform.rotation) as GameObject;
		particleObject.transform.parent = transform;
		particleObject.name = "SwitchParticles" + switchIndex;
		ParticleSystem particles = particleObject.GetComponent<ParticleSystem> ();
		particles.startColor = particleColors [switchIndex % particleColors.Length];
		particles.gameObject.SetActive (activated);

		// Draw a line between the switch and its attached object.
		GameObject lineObject = Instantiate (switchLine) as GameObject;
		lineObject.transform.parent = transform;
		lineObject.name = "LineRenderer" + switchIndex;
		LineRenderer line = lineObject.GetComponent<LineRenderer> ();
		line.SetPosition (0, transform.position);
		line.SetPosition (1, attachedObject.transform.position);
		line.SetColors (particles.startColor, particles.startColor);
	}

	// Update is called once per frame
	protected void Update () {
		LineRenderer line = transform.FindChild ("LineRenderer" + switchIndex).GetComponent<LineRenderer> ();
		line.SetPosition (1, attachedObject.transform.position);
		if(!activated) {
			line.SetColors(Color.black, Color.black);
		} else {
			line.SetColors(new Color(1,0,1), new Color(1,0,1));
		}
	}

	// Turns the switch on or off.
	public virtual void Toggle () {
		activated = !activated;
		transform.FindChild ("SwitchParticles" + switchIndex).gameObject.SetActive (activated);
	}
}
