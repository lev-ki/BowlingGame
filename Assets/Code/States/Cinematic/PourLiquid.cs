using System.Collections;
using UnityEngine;
using DG.Tweening;
using Code.DataContainers;

namespace Code.States.Cinematic
{
    [CreateAssetMenu(fileName = "PourLiquidCinematic", menuName = "SO/GameStates/Cinematic/PourLiquid", order = 0)]
    public class PourLiquid : BaseState
    {
        private static readonly int FillShaderProperty = Shader.PropertyToID("_Fill");

        public override void OnEnter()
        {
            PourLiquidCinematic();
            base.OnEnter();
        }

        private void PourLiquidCinematic()
        {
            float liquidLevel = ProgressionContainer.Instance.currentScore;
            float pouringDuration = liquidLevel * 2f; // magic number, yay
            CinematicObjectsContainer coc = CinematicObjectsContainer.Instance;
            ParticleSystem particleInstance = Instantiate(coc.pourLiquidParticleSystemPrefab, coc.particleSystemParent);
            particleInstance.transform.position = coc.particleSystemParent.position;

            GameManager.Instance.InvokeEvent(EventId.CinematicFinished); // we can do the pouring in the bg behind the scoring UI
            Sequence sequence = DOTween.Sequence()
                .Append(coc.pourLiquidBottle.DOMove(coc.bottlePouringPosition.position, 0.75f))
                .Join(coc.pourLiquidBottle.DORotateQuaternion(coc.bottlePouringPosition.rotation, 0.75f))
                .AppendCallback(() =>
                {
                    var main = particleInstance.main;
                    main.duration = pouringDuration;
                    particleInstance.Play();
                    DOTween.To(() => coc.liquidSource.material.GetFloat(FillShaderProperty),
                               (x) => coc.liquidSource.material.SetFloat(FillShaderProperty, x), 0, pouringDuration);
                })
                .AppendInterval(1f)
                .Append(DOTween.To(() => coc.liquidDestination.material.GetFloat(FillShaderProperty),
                                (x) => coc.liquidDestination.material.SetFloat(FillShaderProperty, x), liquidLevel, pouringDuration))
                .Append(coc.pourLiquidBottle.DOMove(coc.bottleRestingPosition.position, 0.5f))
                .Join(coc.pourLiquidBottle.DORotateQuaternion(coc.bottleRestingPosition.rotation, 0.5f))
                .AppendCallback(() =>
                {
                    Destroy(particleInstance);
                });
        }
    }
}