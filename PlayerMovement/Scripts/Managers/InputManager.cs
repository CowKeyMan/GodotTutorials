using Godot;
using System;
using System.Collections.Generic;

public class InputManager : Node
{
  public static float controllerToMouse = 15.0f;
  public static float controllerDeadzone = 0.1f;

  enum ActionNames { LEFT, RIGHT, UP, DOWN }

  private Dictionary<string, InputEvent[]> nameAction;

  private static Vector2 mousePosition;
  private static Vector2 previousMousePosition;

  private static InputManager inputManager;

  public InputManager()
  {
	PopulateNameAction();
	inputManager = this;
  }

  public override void _Ready()
  {
	mousePosition = Vector2.Zero;
	previousMousePosition = Vector2.Zero;
  }

  private void PopulateNameAction()
  {
	// Initialize the dictionary
	nameAction = new Dictionary<string, InputEvent[]>();

	// TODO: Load from file if already exists

	nameAction[ActionNames.LEFT.ToString()] = new InputEvent[]
	{
	  new InputEventKey(){ Scancode = (int)Godot.KeyList.A },
		  new InputEventJoypadButton(){ ButtonIndex = (int)Godot.JoystickList.DpadLeft }
	};

	nameAction[ActionNames.RIGHT.ToString()] = new InputEvent[]
	{
	  new InputEventKey(){ Scancode = (int)Godot.KeyList.D },
		  new InputEventJoypadButton(){ ButtonIndex = (int)Godot.JoystickList.DpadRight }
	};

	nameAction[ActionNames.DOWN.ToString()] = new InputEvent[]
	{
	  new InputEventKey(){ Scancode = (int)Godot.KeyList.W },
		  new InputEventJoypadButton(){ ButtonIndex = (int)Godot.JoystickList.DpadUp }
	};

	nameAction[ActionNames.UP.ToString()] = new InputEvent[]
	{
	  new InputEventKey(){ Scancode = (int)Godot.KeyList.S },
		  new InputEventJoypadButton(){ ButtonIndex = (int)Godot.JoystickList.DpadDown }
	};

	// Add to the built-in input map itself
	foreach (string key in nameAction.Keys)
	{
	  InputMap.AddAction(key);
	  foreach (InputEvent ev in nameAction[key])
	  {
		InputMap.ActionAddEvent(key, ev);
	  }
	}
  }

  public override void _Input(InputEvent ev)
  {
	if (@ev is InputEventMouseMotion eventMouseMotion){
	  previousMousePosition = mousePosition;
	  mousePosition += eventMouseMotion.Relative;
	}
  }

  public static float GetAxisHorizontal()
  {
	float j = Input.GetJoyAxis(0,0) * 1.414213562f; // Device 0, axis 0
	if(j>1) j=1;
	else if(j<-1) j=-1;
	return Math.Abs(j)>controllerDeadzone?j:0 + (Input.IsActionPressed("RIGHT") ? 1 : 0) - (Input.IsActionPressed("LEFT") ? 1 : 0);
  }

  public static float GetAxisVertical()
  {
	float j = Input.GetJoyAxis(0,1) * 1.414213562f; // Device 0, axis 1
	if(j>1) j=1;
	else if(j<-1) j=-1;
	return Math.Abs(j)>controllerDeadzone?j:0 + (Input.IsActionPressed("UP") ? 1 : 0) - (Input.IsActionPressed("DOWN") ? 1 : 0);
  }

  public static Vector2 GetSecondaryAxis() // Get right stick / mouse position relative
  {
	Vector2 toMove = mousePosition - previousMousePosition;
	float jh = Input.GetJoyAxis(0,2) * 1.414213562f;
	if(jh>1) jh=1;
	else if(jh<-1) jh=-1;
	float jv = Input.GetJoyAxis(0,3) * 1.414213562f;
	if(jv>1) jv=1;
	else if(jv<-1) jv=-1;

	Vector2 controllerInput = new Vector2(Math.Abs(jh)>controllerDeadzone?jh:0, Math.Abs(jv)>controllerDeadzone?jv:0)*controllerToMouse;

	previousMousePosition = mousePosition;

	return controllerInput + toMove;
  }

  public static bool IsJumping() { return Input.IsActionPressed("JUMP"); }

}
