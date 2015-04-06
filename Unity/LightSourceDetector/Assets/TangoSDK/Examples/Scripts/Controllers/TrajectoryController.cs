﻿/*
 * Copyright 2014 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System.Collections;

public class TrajectoryController : MonoBehaviour {

    public SampleController m_sampleController;
    private GameObject m_blueTrajectory;
    private GameObject m_greenTrajectory;
    
    /// <summary>
    /// Used to initialize objects.
    /// </summary>
    void Awake () 
    {
        m_blueTrajectory = GameObject.Find("BlueTrajectory");
        m_greenTrajectory = GameObject.Find("GreenTrajectory");
    }
    
    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update () 
    {
        if(m_sampleController.IsLocalized())
        {
            m_greenTrajectory.transform.position = m_sampleController.transform.position;
        }
        else
        {
            m_blueTrajectory.transform.position = m_sampleController.transform.position;
        }
    }
}
