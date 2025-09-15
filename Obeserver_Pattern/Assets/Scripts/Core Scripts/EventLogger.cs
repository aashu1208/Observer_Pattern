using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLogger : MonoBehaviour, ISubscriber
{
	private GameManager _gameManager;

	void OnEnable()
	{
		_gameManager = FindObjectOfType<GameManager>();
		if (_gameManager != null)
		{
			_gameManager.Subscribe(this);
		}
	}

	void OnDisable()
	{
		if (_gameManager != null)
		{
			_gameManager.Unsubscribe(this);
		}
	}

	public void OnNotify(string eventType, object param)
	{
		Debug.Log($"[EventLogger] Event: {eventType}, Param: {param}");
	}
}


