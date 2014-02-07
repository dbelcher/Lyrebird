﻿using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace LMNA.Lyrebird
{
    public class TaskContainer
    {
        private static readonly object LockObj = new object();
        private volatile static TaskContainer _instance;

        private readonly Queue<Action<UIApplication>> _tasks;

        private TaskContainer()
        {
            _tasks = new Queue<Action<UIApplication>>();
        }

        public static TaskContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new TaskContainer();
                        }
                    }
                }
                return _instance;
            }
        }

        public void EnqueueTask(Action<UIApplication> task)
        {
            lock (_tasks)
            {
                _tasks.Enqueue(task);
            }
        }

        public bool HasTaskToPerform
        {
            get { return _tasks.Count > 0; }
        }

        public Action<UIApplication> DequeueTask()
        {
            return _tasks.Dequeue();
        }
    }
}
