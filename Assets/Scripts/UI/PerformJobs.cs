using Godot;
using System;
using System.Collections.Generic;

public partial class PerformJobs : Control
{
	//potential TODO: create a Dictionary<Job, Goat[]> to avoid looping too many times
	VBoxContainer jobListContainer;
	PackedScene jobRow = GD.Load<PackedScene>("res://Scenes/UI/JobRow.tscn");
	List<Node> rows = new List<Node>();

	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowPerformJobs, Callable.From(()=> OnShowPerformJobs()), (uint)ConnectFlags.Deferred);

		jobListContainer = GetNode<VBoxContainer>("MainContainer/MainVBoxContainer/MainHBox/JobListMargin/JobListContainer");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void DoJobs()
	{
		//TODO: 
			// loop through the goats and calculate their contributions
			// take 10 seconds, run through the goats 3 times each (may need to balance)
			// add animations

		GD.Print("Doing jobs");
	}

	private void OnShowPerformJobs()
	{
		Show();

		InstantiateChildren();
		
		//TODO: finished button grabs focus AFTER becoming visible
		//cancelButton.CallDeferred("grab_focus");
	}

	private void InstantiateChildren()
	{
		//remove all of the old children
		foreach(Node child in jobListContainer.GetChildren())
		{
			jobListContainer.RemoveChild(child);
			child.QueueFree();
		}

		//spawn in new jobs
		foreach(Job job in GlobalVars.jobs)
		{
			//spawn the job row
			Node row = jobRow.Instantiate();
			SetRowInformation(job, row);
			jobListContainer.AddChild(row);
		}
	}

	private void SetRowInformation(Job job, Node row)
	{
		Label nameLabel = row.GetNode<Label>("JobMargin/JobHBox/JobNameMargin/JobVBox/NameLabel");
		Label resultsLabel = row.GetNode<Label>("JobMargin/JobHBox/ResultsMargin/ResultsHBox/ResultsLabel"); //TODO: UNUSED
		nameLabel.Text = job.Name;

		rows.Add(row); //add the Node to the list of rows for access later... POTENTIAL TODO: dictionary<Job, Node>
		row.Name = job.ID.ToString();

	}
}
