using System.ComponentModel;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid.Data;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Interfaces;
using Ostryzhnyi.DI;
using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.DI
{
    public class BrickPuzzleInstaller : MonoInstaller
    {
        [SerializeField] private FieldGrid _fieldGrid = default;
        [SerializeField] private GridSettings _gridSettings = default;
        
        
        protected override void Register()
        {
            RegisterGameplayServices();
        }


        private void RegisterGameplayServices()
        {
            Container.Register<ISizeFieldElement>(_fieldGrid);
            Container.Register<FieldGrid>(_fieldGrid);
            Container.Register<GridSettings>(_gridSettings);
        }
    }
}