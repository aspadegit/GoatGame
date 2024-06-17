using Godot;
using System;
using System.Collections.Generic;

public partial class PerformJobs : Control
{
	VBoxContainer jobListContainer;
	PackedScene jobRow = GD.Load<PackedScene>("res://Scenes/UI/RowEntries/JobRow.tscn");
	Dictionary<int, Node> rows; // (jobID, row)
	Dictionary<int, Dictionary<Material, int>> currentSessionResults; // JobID, (material, amount)
	Timer timer;
	TextureButton closeButton;
	int timerCount = 0;
	public override void _Ready()
	{
		SignalHandler.Instance.Connect(SignalHandler.SignalName.ShowPerformJobs, Callable.From(()=> OnShowPerformJobs()), (uint)ConnectFlags.Deferred);

		jobListContainer = GetNode<VBoxContainer>("MainContainer/MainVBoxContainer/MainHBox/JobListMargin/ScrollContainer/JobListContainer");
		timer = GetNode<Timer>("Timer");
		closeButton = GetNode<TextureButton>("MainContainer/MainVBoxContainer/ButtonHBox/CloseButton");
		closeButton.Pressed += HideAndReset;

		rows = new Dictionary<int, Node>();
		currentSessionResults = new Dictionary<int, Dictionary<Material, int>>();
	}

	public void OnTimerTimeout()
	{
		//get goat resources
		DoJobs();

		if(timerCount >= 4)
		{
			timer.Stop();
			closeButton.Show();
		}
		timerCount++;

	}

	private void DoJobs()
	{
		//TODO: 
			// add animations, balancing
		
		foreach(KeyValuePair<int, Goat> goatPair in GlobalVars.goats)
		{
			Goat curGoat = goatPair.Value;
			CalculateGoatContribution(curGoat);

		}
		
	}
	private void CalculateGoatContribution(Goat goat)
	{
		Job job = goat.AssignedJob;

		//calculate contributions
		foreach(KeyValuePair<Material, int> result in job.Result)
		{
			int avgAmount = result.Value;
			int actualAmount = GoatMath(avgAmount, goat);
			Material mat = result.Key;
			
			//TODO: move the globalvars to update at the end of it all
			//already has the key, just update the value
			if(GlobalVars.materialsObtained.ContainsKey(mat))
			{
				GlobalVars.materialsObtained[mat] += actualAmount; 	//overall

			}
			//does not have the key, add it
			else
			{
				GlobalVars.materialsObtained.Add(mat, actualAmount);	//overall
			}

			//same as above, but ties it to a job
			if(currentSessionResults[job.ID].ContainsKey(mat))
				currentSessionResults[job.ID][mat] += actualAmount;
			
			else
				currentSessionResults[job.ID].Add(mat, actualAmount);

		}
		SetJobVisualResults(job.ID);
	}
	private int GoatMath(int avgAmount, Goat goat)
	{
		//TODO: refine the goat math
		int result = avgAmount;

		result *= goat.Level;
		//TODO: adjust based on class / other stats
		result += (int)goat.Stamina/20;
		
		goat.DoJob(1, 5);

		return result;
	}
	private void SetJobVisualResults(int jobID)
	{
		Node row = rows[jobID];
		Dictionary<Material, int> results = currentSessionResults[jobID];

		Label resultsLabel = row.GetNode<Label>("JobMargin/JobHBox/ResultsMargin/ResultsHBox/ResultsLabel"); 
		string resultText = "RESULTS: ";

		//update the values
		foreach(KeyValuePair<Material, int> materialPair in results)
		{
			resultText += "[ ";
			resultText += materialPair.Key.Name;
			resultText += " | ";
			resultText += materialPair.Value;
			resultText += "] ";
		}
		resultsLabel.Text = resultText;

	}

	private void OnShowPerformJobs()
	{
		Show();

		InstantiateChildren();
		timer.Start();
		//TODO: finished button grabs focus AFTER becoming visible
		//cancelButton.CallDeferred("grab_focus");
	}

	private void HideAndReset()
	{
		Hide();
		closeButton.Hide();
		currentSessionResults.Clear();
		rows.Clear();
		timerCount = 0;
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
		foreach(KeyValuePair<int, Job> jobPair in GlobalVars.jobs)
		{
			Job job = jobPair.Value;
			//spawn the job row
			Node row = jobRow.Instantiate();
			SetRowInformation(job, row);
			jobListContainer.AddChild(row);
			currentSessionResults.Add(job.ID, new Dictionary<Material, int>());
		}
		
	}

	private void SetRowInformation(Job job, Node row)
	{
		row.Name = job.ID.ToString();
		Label nameLabel = row.GetNode<Label>("JobMargin/JobHBox/JobNameMargin/JobVBox/NameLabel");
		nameLabel.Text = job.Name;

		rows.Add(job.ID, row); //add the Node to the list of rows for access later...
		row.Name = job.ID.ToString();

	}
}
