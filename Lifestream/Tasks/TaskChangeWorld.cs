﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lifestream.Schedulers;

namespace Lifestream.Tasks
{
    internal static unsafe class TaskChangeWorld
    {
        internal static void Enqueue(string world)
        {
            P.TaskManager.Enqueue(WorldChange.TargetValidAetheryte);
            P.TaskManager.Enqueue(WorldChange.InteractWithTargetedAetheryte);
            P.TaskManager.Enqueue(WorldChange.SelectVisitAnotherWorld);
            P.TaskManager.Enqueue(() => WorldChange.SelectWorldToVisit(world));
            P.TaskManager.Enqueue(() => WorldChange.ConfirmWorldVisit(world));
        }
    }
}
