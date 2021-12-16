using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ballistics;
using System;

namespace Ballistics
{
    /*Work in progress script for visualizing the trajectory*/
    public class Trajectory : MonoBehaviour
    {

        public Weapon myWeapon;
        public float trajectoryDistance;
        public int trajectoryResolution;
        public Color trajectoryColor;

        public float zeroingDist;

        private Vector3[] trajectoryPoints;
        private BallisticSettings settings;

        private void Start()
        {
            settings = BulletHandler.instance.GetSettings();
        }

        private void LateUpdate()
        {
            UpdateTrajectoryPoints(myWeapon.PhysicalBulletSpawnPoint.forward);
            DrawTrajectory();
        }

        private void DrawTrajectory()
        {
            Vector3 startPos = myWeapon.PhysicalBulletSpawnPoint.position;
            Vector3 currentPos;
            Vector3 lastPos = trajectoryPoints[0] + startPos;

            for (int i = 0; i < trajectoryPoints.Length - 1; i++)
            {
                currentPos = trajectoryPoints[i + 1] + startPos;
                Debug.DrawLine(lastPos, currentPos, trajectoryColor);
                lastPos = currentPos;
            }
        }

        public void UpdateTrajectoryPoints()
        {
            UpdateTrajectoryPoints(myWeapon.PhysicalBulletSpawnPoint.forward);
        }

        public void UpdateTrajectoryPoints(Vector3 dir)
        {
            dir.Normalize();

            Vector3 right = Vector3.Cross(dir, Vector3.up);
            float zeroAngle = zeroingDist > 0 ? myWeapon.calculateZeroingCorrectionAngle(zeroingDist, settings.useBulletdrag, settings.AirDensity) : 0;
            dir = Quaternion.AngleAxis(zeroAngle, right) * dir;

            trajectoryPoints = new Vector3[trajectoryResolution + 1];
            trajectoryPoints[0] = Vector3.zero;

            float stepDist = trajectoryDistance / (trajectoryResolution + 1);
            Vector3 down = Physics.gravity.normalized;

            for (int i = 1; i < trajectoryPoints.Length; i++)
            {
                float dist = stepDist * i;
                float drop = myWeapon.calculateBulletdrop(dist, settings.useBulletdrag, settings.AirDensity);
                trajectoryPoints[i] = dir * dist +  down * drop;
            }
        }
    }
}