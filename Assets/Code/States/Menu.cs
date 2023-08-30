using Code.DataContainers;
using UnityEngine;

namespace Code.States
{
    [CreateAssetMenu(fileName = "Menu", menuName = "SO/GameStates/Menu", order = 0)]
    public class Menu : BaseState
    {
        public override void OnEnter()
        {
            //UIContainer.Instance.menuController.enabled = true;
            UIContainer.Instance.gameName3d.SetActive(true);
            CinematicObjectsContainer.Instance.musicSource.Stop();
            CinematicObjectsContainer.Instance.musicSource.clip = CinematicObjectsContainer.Instance.menuMusic;
            CinematicObjectsContainer.Instance.musicSource.Play();
        }

        public override void OnExit()
        {
            UIContainer.Instance.gameName3d.SetActive(false);
            //UIContainer.Instance.menuController.enabled = false;
        }
    }
}