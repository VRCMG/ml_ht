namespace ml_ht
{
    public class HeadTurn : MelonLoader.MelonMod
    {
        static bool ms_update = false;

        float m_lockedBodyRotation = 0f;
        static bool ms_lockBodyRotation = false;

        public override void OnUpdate()
        {
            if(ms_update)
            {
                bool l_ignoreLimit = UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl);

                if(UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftAlt))
                {
                    float l_angle = UnityEngine.Input.mouseScrollDelta.y;
                    if(l_angle != 0f)
                    {
                        var l_neckRotator = VRCPlayer.field_Internal_Static_VRCPlayer_0?.gameObject?.GetComponent<GamelikeInputController>()?.field_Protected_NeckMouseRotator_0;
                        if(l_neckRotator != null)
                        {
                            var l_quat = l_neckRotator.field_Private_Quaternion_0;
                            var l_vec = l_quat.eulerAngles;
                            l_vec.z += l_angle * 5f;
                            if(!l_ignoreLimit)
                            {
                                float l_delta = UnityEngine.Mathf.DeltaAngle(l_vec.z, 0f);
                                if(UnityEngine.Mathf.Abs(l_delta) > 90f)
                                {
                                    l_vec.z += (UnityEngine.Mathf.Abs(l_delta) - 90f) * UnityEngine.Mathf.Sign(l_delta);
                                    l_vec.z = (l_vec.z + 360f) % 360f;
                                }
                            }
                            l_quat.eulerAngles = l_vec;
                            l_neckRotator.field_Private_Quaternion_0 = l_quat;
                        }
                    }
                }

                if(UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftAlt))
                {
                    if(!ms_lockBodyRotation)
                    {
                        var l_transform = VRCPlayer.field_Internal_Static_VRCPlayer_0?.prop_VRCAvatarManager_0?.gameObject?.transform;
                        if(l_transform != null)
                        {
                            m_lockedBodyRotation = l_transform.rotation.eulerAngles.y;
                            ms_lockBodyRotation = true;
                        }
                    }
                }
                if(UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.LeftAlt) || !UnityEngine.Application.isFocused)
                {
                    if(ms_lockBodyRotation)
                    {
                        var l_transformPlayer = VRCPlayer.field_Internal_Static_VRCPlayer_0?.gameObject?.transform;
                        var l_transformAvatar = VRCPlayer.field_Internal_Static_VRCPlayer_0?.prop_VRCAvatarManager_0?.gameObject?.transform;
                        if(l_transformAvatar != null && l_transformAvatar != null)
                        {
                            l_transformAvatar.rotation = l_transformPlayer.rotation;
                        }
                        ms_lockBodyRotation = false;
                    }
                }
                if(ms_lockBodyRotation)
                {
                    var l_playerTransform = VRCPlayer.field_Internal_Static_VRCPlayer_0?.gameObject?.transform;
                    var l_animatorTransform = VRCPlayer.field_Internal_Static_VRCPlayer_0?.prop_VRCAvatarManager_0?.gameObject?.transform;
                    if((l_playerTransform != null) && (l_animatorTransform != null))
                    {
                        if(!l_ignoreLimit)
                        {
                            float l_delta = UnityEngine.Mathf.DeltaAngle(m_lockedBodyRotation, l_playerTransform.eulerAngles.y);
                            if(UnityEngine.Mathf.Abs(l_delta) > 90f)
                            {
                                m_lockedBodyRotation += (UnityEngine.Mathf.Abs(l_delta) - 90f) * UnityEngine.Mathf.Sign(l_delta);
                                m_lockedBodyRotation = (m_lockedBodyRotation + 360f) % 360f;
                            }
                        }

                        var l_quat = l_animatorTransform.rotation;
                        var l_vec = l_quat.eulerAngles;
                        l_vec.y = m_lockedBodyRotation;
                        l_quat.eulerAngles = l_vec;
                        l_animatorTransform.rotation = l_quat;
                    }
                }
            }
        }

        // Join patch
        [HarmonyLib.HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.OnJoinedRoom))]
        class Patch_NetworkManager_OnJoinedRoom
        {
            static void Postfix() => ms_update = true;
        }

        // Left patch
        [HarmonyLib.HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.OnLeftRoom))]
        class Patch_NetworkManager_OnOnLeftRoom
        {
            static void Postfix()
            {
                ms_update = false;
                ms_lockBodyRotation = false;
            }
        }
    }
}
