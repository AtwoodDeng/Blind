using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// The list of logic events
/// </summary>
public enum LogicEvents
{
	None=0,
	Interact = 1,
	SwitchBGM = 2,
	SwitchDefaultBGM = 3,
	EnterSavePoint = 4,
	LockCamera = 5,
	UnlockCamera = 6,
	PauseGame = 7,
	UnpauseGame = 8,

	/// <summary>
	/// call to start the death effect
	/// </summary>
	Death = 10,
	DeathEnd = 11,

	BeginDamage = 20,
	EndDamage = 21,
	BeginRun = 22,
	EndRun = 23,

	DisplayDialog = 30,
	EndDisplayDialog = 31,
	DisplayNextDialog = 32,

	DisplayThought = 40,
	SwitchThoughtBox = 41,

	FocusCamera = 42,
	UnfocusCamera = 43,

	HideThought = 45,

	PlayMusic = 50,

	Sneeze = 60,
	Breath = 70,

	EndGame = 99,

	EndTalkManInCafe = 100,
	EnterStreetOne = 101,
	EnterRotateBuilding = 102,
	EnterStreetTwo = 103,
	EnterStreetThree = 104,
	EnterTakePhoto = 105,
	SeeGirlStreetTwo = 106,
	EnterBorrowUmbrella = 107,
	EnterStreetThreeEnd = 108,

	EnterBusStop = 110,
	BusStopEndTalkGirl = 111,
	BustStopTalkPointOne = 112,
	BustStopTalkPointTwo = 113,

	TrafficRedLight = 120,
	TrafficGreenLight = 121,
	InvisibleFromPlayer = 122,
	EnterStreetFour = 123,
	EnterStreetFourEnd = 124,
	ForceGirlLeave = 125,
	GirlSayPlayMusic = 126,
	GirlJudgeMusic = 127,

	EnterStreetColorful = 130,
	EndTalkWithGirl = 131,
	ExitStreetColorful = 132,
	EnterApartment = 133,

	OpenApartmentDoor = 134,
	WalkInApartment = 135,
	EndHitOne = 136,
	EndHitTwo = 137,
	EndHitThree = 138,
	EndHitFour = 139,
	DisPlayClimaxEffect = 141,
	PickUpMusicPlayer = 142,
	EnterStone = 143,

	PlayEndBGM = 145,


	EnterEnd = 140,



	WatchShipOne = 150,
	WatchShipTwo = 151,
	WatchMusicPlayer = 152,

	EnterColorfulClimaxOne = 155,
	EnterColorfulClimaxTwo = 156,
	EnterColorfulClimaxThree = 157,
	EndTalkWithFakeGirl = 158,

	CloseApartment = 159,


	EndCredit = 160,

	FilmShotOne = 180,
	FilmShotTwo = 181,
	FilmShotThree = 182,
	FilmShotFour = 183,
	FilmShotFive = 184,
	FilmShotSix = 185,
	FilmShotSeven = 186,
	FilmShotEight = 187,
	FilmShotNine = 188,

}

public class M_Event : MonoBehaviour {

	/// <summary>
	/// Event handler. handle the event with basic arg
	/// </summary>
	public delegate void EventHandler(BasicArg arg);

	/// <summary>
	/// an example for the normal event
	/// </summary>
//	public static event EventHandler StartApp;
//	public static void FireStartApp(BasicArg arg){if ( StartApp != null ) StartApp(arg) ; }


	/// <summary>
	/// Handler specified for dealing with the logic events
	/// </summary>
	public delegate void LogicHandler( LogicArg arg );

	/// <summary>
	/// The list of logic events, we assum that the number of total events is less than 9999
	/// </summary>
	//	public static LogicHandler[] logicEvents = new LogicHandler[System.Enum.GetNames (typeof (LogicEvents)).Length];
	public static LogicHandler[] logicEvents = new LogicHandler[9999];

	/// <summary>
	/// A static interface to fire the logic events, without specified type
	/// </summary>
	/// <param name="arg">Argument.</param>
	public static void FireLogicEvent( LogicArg arg )
	{
		if (arg.type != LogicEvents.None) {
			FireLogicEvent (arg.type, arg);
		}
	}
	/// <summary>
	/// a static interface to fire the logic events, with specified type
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="arg">Argument.</param>
	public static void FireLogicEvent( LogicEvents type , LogicArg arg )
	{
		if ( logicEvents[(int)type] != null )
		{
			arg.type = type;
			logicEvents [(int)type] ( arg );
		}

	}

	/// <summary>
	/// Registers the event according to the event type
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="handler">Handler.</param>
	public static void RegisterEvent( LogicEvents type , LogicHandler handler )
	{
		logicEvents [(int)type] += handler;
	}

	/// <summary>
	/// Unregisters the event.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="handler">Handler.</param>
	public static void UnregisterEvent( LogicEvents type , LogicHandler handler )
	{
		logicEvents [(int)type] -= handler;
	}

	/// <summary>
	/// Register the handler function to all events
	/// </summary>
	/// <param name="handler">Handler.</param>
	public static void RegisterAll( LogicHandler handler )
	{
		for (int i = 0; i < logicEvents.Length; ++i)
			logicEvents [i] += handler;
	}

	/// <summary>
	/// Unregister the handler to all events
	/// </summary>
	/// <param name="handler">Handler.</param>
	public static void UnRegisterAll ( LogicHandler handler )
	{
		for (int i = 0; i < logicEvents.Length; ++i)
			logicEvents [i] -= handler;
	}

	// const string for message
	public const string EVENT_DISPLAY_DIALOG_PLOT = "KEY";
	public const string EVENT_SWITCH_BGM_CLIP = "CLIP";
	public const string EVENT_SAVE_POINT = "SAVE";
	public const string EVENT_THOUGHT = "THOUGHT";
	public const string EVENT_END_DISPLAY_SENDER = "SENDER";
	public const string EVENT_END_DISPLAY_FRAME = "IMPORTANT";
	public const string EVENT_PLAY_MUSIC_NAME = "MUSIC_NAME";
	public const string EVENT_BGM_FADE_TIME = "BGM_FADE_TIME";


}

/// <summary>
/// Basic argument class, with a sender 
/// </summary>
public class BasicArg : EventArgs
{
	public BasicArg(object _this){m_sender = _this;}
	object m_sender;
	public object sender{get{return m_sender;}}
}

/// <summary>
/// Message argument, with a dictionary to record the parameters
/// </summary>
public class MsgArg : BasicArg
{
	public MsgArg(object _this):base(_this){}
	Dictionary<string,object> m_dict;
	Dictionary<string,object> dict
	{
		get {
			if ( m_dict == null )
				m_dict = new Dictionary<string, object>();
			return m_dict;
		}
	}
	public void AddMessage(string key, object val)
	{
		dict.Add(key, val);
	}
	public object GetMessage(string key)
	{
		object res;
		dict.TryGetValue(key , out res);
		return res;
	}
	public bool ContainMessage(string key)
	{
		return dict.ContainsKey(key);
	}
}

/// <summary>
/// Logic argument.
/// </summary>
public class LogicArg : MsgArg
{
	public LogicArg(object _this):base(_this){}

	/// <summary>
	/// The type of the arg.
	/// </summary>
	public LogicEvents type;
}