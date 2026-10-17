using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider2D))]
    public class DeathDetector : MonoBehaviour
    {
        [SerializeField] private StateController stateController;
        private Core core;

        [SerializeField] private List<string> hostileTags;

        private Collider2D col;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void Start()
        {
            //Referência ao core precisa ser feita no Start, pois é inicializado no Awake
            core = (Player.Core)stateController.GetCore();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hostileTags.Contains(collision.tag))
            {
                col.enabled = false;

                Vector2 direction = (collision.transform.position - transform.position).normalized;
                core.deathDirection = direction;

                stateController.SetState<DeathState>(false);
            }
        }
    }
}