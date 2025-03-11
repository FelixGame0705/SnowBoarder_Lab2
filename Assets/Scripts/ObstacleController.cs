using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts
{
    public class ObstacleController : MonoBehaviour
    {
        [SerializeField]
        private ScreenShakeProfile screenShakeProfile;

        private CinemachineImpulseSource _impulseSource;

        private void Start()
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                CameraShakeManager.instance.ScreenShakeFromProfile(
                    screenShakeProfile,
                    _impulseSource
                );
            }
        }
    }
}
