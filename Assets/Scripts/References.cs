using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class References
{
    public static PlayerBehavior thePlayer;
    public static GameObject canvas;
    public static bool isPaused;
    public static int startingEnergyCapsuleCount;
    public static int currentEnergyCapsuleCount;
    public static LevelBehavior theLevelLogic;
	public static List<YeetTriggerBehavior> allYeetTriggers = new List<YeetTriggerBehavior>();
	public static NRGTrackerBehavior playerNRGTracker;
	public static PauseMenuBehavior thePauseMenu;
	public static TextMeshProUGUI mainMenuExitPrompt;
	public static float yeeto = 0;
	public static TimerBarBehavior theTimerBar;
	public static CheckpointBehavior defaultCheckpoint;
	public static List<CheckpointBehavior> Checkpoints = new List<CheckpointBehavior>();
	public static int AddCheckpoint(CheckpointBehavior checkpoint)
	{
		Checkpoints.Add(checkpoint);
		return Checkpoints.IndexOf(checkpoint);
	}

	public const string jump = "Jump";
	public const string yeet = "Attack";
	public const string skate = "Skate";
	public const string forward = "Forward";
	public const string backward = "Backward";
	public const string left = "Left";
	public const string right = "Right";
	public const string grab = "Grab";
}