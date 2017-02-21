namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class CollisionDetectPattern
    {
        private static RaycastHit _castToAtMost = new RaycastHit();
        private static Collider[] _collidersBuffer = new Collider[0x40];
        private static RaycastHit _hitscanHit;
        private static RaycastHit[] _raycastHitsBuffer = new RaycastHit[0x40];
        private const int NON_ALLOC_CACHE_SIZE = 0x40;

        public static bool CenterHit(Vector3 origin, Collider collider, out RaycastHit castHit)
        {
            Vector3 vector = collider.ClosestPointOnBounds(origin);
            if (vector == origin)
            {
                castHit = new RaycastHit();
                return false;
            }
            Vector3 direction = vector - origin;
            direction.y = 0f;
            if (direction == Vector3.zero)
            {
                castHit = new RaycastHit();
                return false;
            }
            Ray ray = new Ray(origin, direction);
            return collider.Raycast(ray, out castHit, float.MaxValue);
        }

        public static bool CheckCapsule(Vector3 startPt, Vector3 endPt, float radius, LayerMask mask)
        {
            return Physics.CheckCapsule(startPt, endPt, radius, (int) mask);
        }

        private static bool CheckStartingPointInsideness(Collider collider, Vector3 startingPoint, Vector3 forward, List<CollisionResult> outResults, out RaycastHit hit)
        {
            Vector3 direction = collider.bounds.center - startingPoint;
            Ray ray = new Ray(startingPoint, direction);
            if (collider.Raycast(ray, out hit, direction.magnitude))
            {
                return false;
            }
            BaseMonoEntity componentInParent = collider.gameObject.GetComponentInParent<BaseMonoEntity>();
            if (!IsEntityAlreadyInResults(outResults, componentInParent))
            {
                outResults.Add(new CollisionResult(componentInParent, collider.ClosestPointOnBounds(startingPoint), forward));
            }
            return true;
        }

        public static List<CollisionResult> CircleCollisionDetectBySphere(Vector3 circleCenterPoint, float offsetZ, Vector3 direction, float radius, LayerMask mask)
        {
            return FanCollisionDetectBySphere(circleCenterPoint, offsetZ, radius, direction, 360f, mask);
        }

        public static List<CollisionResult> CylinderCollisionDetectBySphere(Vector3 hitCastFromCenter, Vector3 cylinderCenterPoint, float radius, float height, LayerMask mask)
        {
            List<CollisionResult> results = new List<CollisionResult>();
            if (radius > 0f)
            {
                int num = Physics.SphereCastNonAlloc(cylinderCenterPoint - ((Vector3) (radius * Vector3.up)), radius, Vector3.up, _raycastHitsBuffer, radius + height, (int) mask);
                for (int i = 0; i < num; i++)
                {
                    RaycastHit hit = _raycastHitsBuffer[i];
                    Collider collider = hit.collider;
                    BaseMonoEntity componentInParent = collider.gameObject.GetComponentInParent<BaseMonoEntity>();
                    if ((componentInParent != null) && !IsEntityAlreadyInResults(results, componentInParent))
                    {
                        RaycastHit hit2;
                        Vector3 origin = hitCastFromCenter;
                        origin.y = height * 0.5f;
                        if (!CenterHit(origin, collider, out hit2))
                        {
                            hit2 = hit;
                            Vector3 vector2 = hitCastFromCenter - hit2.point;
                            vector2.y = 0f;
                            hit2.normal = vector2;
                        }
                        results.Add(new CollisionResult(componentInParent, hit2.point, -hit2.normal));
                    }
                }
            }
            return results;
        }

        public static List<CollisionResult> FanCollisionDetectBySphere(Vector3 fanCenterPoint, float offsetZ, float radius, Vector3 direction, float fanAngle, LayerMask mask)
        {
            List<CollisionResult> results = new List<CollisionResult>();
            if (radius > 0.0)
            {
                Vector3 forward = direction;
                forward.y = 0f;
                forward.Normalize();
                Vector3 position = fanCenterPoint + ((Vector3) (offsetZ * forward.normalized));
                int num = Physics.OverlapSphereNonAlloc(position, radius, _collidersBuffer, (int) mask);
                for (int i = 0; i < num; i++)
                {
                    RaycastHit hit;
                    Collider collider = _collidersBuffer[i];
                    BaseMonoEntity componentInParent = collider.gameObject.GetComponentInParent<BaseMonoEntity>();
                    if (!IsEntityAlreadyInResults(results, componentInParent) && !CheckStartingPointInsideness(collider, position, forward, results, out hit))
                    {
                        Vector3 vector3 = collider.bounds.center - position;
                        vector3.y = 0f;
                        Ray ray = new Ray(position, vector3);
                        if (collider.Raycast(ray, out hit, radius) && (Vector3.Angle(forward, vector3) <= (fanAngle * 0.5f)))
                        {
                            Vector3 hitForward = -hit.normal;
                            hitForward.y = 0f;
                            results.Add(new CollisionResult(componentInParent, hit.point, hitForward));
                        }
                        else
                        {
                            int num4;
                            if (radius < InLevelData.MIN_COLLIDER_RADIUS)
                            {
                                num4 = 1;
                            }
                            else
                            {
                                num4 = Mathf.CeilToInt((fanAngle * 0.01745329f) / (2f * Mathf.Asin(InLevelData.MIN_COLLIDER_RADIUS / radius))) + 1;
                            }
                            float angle = fanAngle / ((float) (num4 - 1));
                            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
                            Vector3 vector5 = (Vector3) (Quaternion.AngleAxis(0.5f * -fanAngle, Vector3.up) * forward);
                            for (int j = 0; j < num4; j++)
                            {
                                ray.direction = vector5;
                                if (collider.Raycast(ray, out hit, radius))
                                {
                                    RaycastHit hit2;
                                    if (!CenterHit(position, collider, out hit2))
                                    {
                                        hit2 = hit;
                                    }
                                    results.Add(new CollisionResult(componentInParent, hit2.point, vector5));
                                    break;
                                }
                                vector5 = (Vector3) (quaternion * vector5);
                            }
                        }
                    }
                }
            }
            return results;
        }

        public static Vector3 GetNearestHitPoint(BaseMonoEntity entity, Vector3 center)
        {
            if (entity is BaseMonoAvatar)
            {
                BaseMonoAvatar avatar = (BaseMonoAvatar) entity;
                return avatar.hitbox.ClosestPointOnBounds(center);
            }
            if (entity is BaseMonoMonster)
            {
                BaseMonoMonster monster = (BaseMonoMonster) entity;
                return monster.hitbox.ClosestPointOnBounds(center);
            }
            Vector3 xZPosition = entity.XZPosition;
            xZPosition.y = 1f;
            return xZPosition;
        }

        public static float GetRaycastDistance(Vector3 origin, Vector3 forward, float maxDistance, float minClipOff, LayerMask mask)
        {
            float num = maxDistance;
            if (Physics.Raycast(origin, forward, out _castToAtMost, maxDistance + minClipOff, (int) mask))
            {
                return (_castToAtMost.distance - minClipOff);
            }
            return maxDistance;
        }

        public static Vector3 GetRaycastPoint(Vector3 origin, Vector3 forward, float maxDistance, float minClipOff, LayerMask mask)
        {
            forward.Normalize();
            if (Physics.Raycast(origin, forward, out _castToAtMost, maxDistance + minClipOff, (int) mask))
            {
                return (origin + ((Vector3) (forward * (_castToAtMost.distance - minClipOff))));
            }
            return (origin + ((Vector3) (forward * maxDistance)));
        }

        public static List<CollisionResult> HitscanSingleDetect(Vector3 rayCenterPoint, Vector3 rayForward, float maxDistance, LayerMask mask)
        {
            List<CollisionResult> list = new List<CollisionResult>();
            rayForward.Normalize();
            if (Physics.Raycast(rayCenterPoint, rayForward, out _hitscanHit, maxDistance, (int) mask))
            {
                BaseMonoEntity componentInParent = _hitscanHit.collider.GetComponentInParent<BaseMonoEntity>();
                list.Add(new CollisionResult(componentInParent, _hitscanHit.point, rayForward));
                return list;
            }
            if (Physics.Raycast(rayCenterPoint + ((Vector3) (rayForward * maxDistance)), -rayForward, out _hitscanHit, maxDistance, (int) mask))
            {
                BaseMonoEntity entity = _hitscanHit.collider.GetComponentInParent<BaseMonoEntity>();
                list.Add(new CollisionResult(entity, rayCenterPoint, rayForward));
            }
            return list;
        }

        private static bool IsEntityAlreadyInResults(List<CollisionResult> results, BaseMonoEntity entity)
        {
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].entity.GetRuntimeID() == entity.GetRuntimeID())
                {
                    return true;
                }
            }
            return false;
        }

        public static int[] MakeCenteredIndices(int count)
        {
            int[] numArray = new int[count];
            int num = count % 2;
            int num2 = count / 2;
            if (num == 1)
            {
                numArray[0] = num2;
                for (int j = 0; j < num2; j++)
                {
                    numArray[(2 * j) + 1] = (num2 - j) - 1;
                    numArray[(2 * j) + 2] = (num2 + j) + 1;
                }
                return numArray;
            }
            for (int i = 0; i < num2; i++)
            {
                numArray[2 * i] = (num2 - 1) - i;
                numArray[(2 * i) + 1] = num2 + i;
            }
            return numArray;
        }

        public static List<CollisionResult> MeleeFanCollisionDetectBySphere(Vector3 fanCenterPoint, float offsetZ, Vector3 direction, float radius, float fanAngle, float meleeRadius, float meleeFanAgele, LayerMask mask)
        {
            List<CollisionResult> list = null;
            List<CollisionResult> list2 = null;
            list = FanCollisionDetectBySphere(fanCenterPoint, offsetZ, radius, direction, fanAngle, mask);
            list2 = FanCollisionDetectBySphere(fanCenterPoint, offsetZ, meleeRadius, direction, meleeFanAgele, mask);
            List<CollisionResult> list3 = list;
            for (int i = 0; i < list2.Count; i++)
            {
                CollisionResult item = list2[i];
                bool flag = false;
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].entity.GetRuntimeID() == item.entity.GetRuntimeID())
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list3.Add(item);
                }
            }
            return list3;
        }

        public static List<CollisionResult> RectCollisionDetectByRay(Vector3 recCenterPoint, float offsetZ, Vector3 direction, float width, float distance, LayerMask mask)
        {
            Vector3 vector = direction;
            vector.y = 0f;
            vector.Normalize();
            Vector3 position = recCenterPoint + ((Vector3) (offsetZ * vector.normalized));
            List<CollisionResult> outResults = new List<CollisionResult>();
            int num = Physics.OverlapSphereNonAlloc(position, width, _collidersBuffer, (int) mask);
            for (int i = 0; i < num; i++)
            {
                RaycastHit hit;
                CheckStartingPointInsideness(_collidersBuffer[i], position, direction, outResults, out hit);
            }
            int count = Mathf.CeilToInt(width / (InLevelData.MIN_COLLIDER_RADIUS * 2f)) + 1;
            float num4 = width / ((float) (count - 1));
            Vector3 vector7 = new Vector3(-vector.z, 0f, vector.x);
            Vector3 normalized = vector7.normalized;
            Vector3 vector4 = ((Vector3) ((0.5f * -width) * normalized)) + position;
            Vector3 vector5 = (Vector3) (normalized * num4);
            int[] numArray = MakeCenteredIndices(count);
            for (int j = 0; j < count; j++)
            {
                Vector3 origin = vector4 + ((Vector3) (numArray[j] * vector5));
                num = Physics.RaycastNonAlloc(origin, vector, _raycastHitsBuffer, distance, (int) mask);
                if (num == 0)
                {
                    num = Physics.RaycastNonAlloc(origin + ((Vector3) (vector * distance)), -vector, _raycastHitsBuffer, distance, (int) mask);
                }
                for (int k = 0; k < num; k++)
                {
                    RaycastHit hit2 = _raycastHitsBuffer[k];
                    Collider collider = hit2.collider;
                    BaseMonoEntity componentInParent = collider.gameObject.GetComponentInParent<BaseMonoEntity>();
                    if ((componentInParent != null) && !IsEntityAlreadyInResults(outResults, componentInParent))
                    {
                        RaycastHit hit3;
                        if (!CenterHit(recCenterPoint, collider, out hit3))
                        {
                            hit3 = hit2;
                        }
                        outResults.Add(new CollisionResult(componentInParent, hit3.point, vector));
                    }
                }
            }
            return outResults;
        }

        public static bool SphereOverlapWithEntity(Vector3 centerPoint, float radius, LayerMask mask, GameObject gameobjet)
        {
            int num = Physics.OverlapSphereNonAlloc(centerPoint, radius, _collidersBuffer, (int) mask);
            for (int i = 0; i < num; i++)
            {
                Collider collider = _collidersBuffer[i];
                if (collider.gameObject != gameobjet)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

