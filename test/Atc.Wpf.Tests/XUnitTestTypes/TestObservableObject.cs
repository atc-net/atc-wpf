using System.Diagnostics.CodeAnalysis;
using Atc.Wpf.Mvvm;

namespace Atc.Wpf.Tests.XUnitTestTypes
{
    public class TestObservableObject : ObservableObject
    {
        private bool isBoolProperty;
        private bool isBoolPropertyWithExpression;
        private bool isBoolPropertyWithSet;
        private bool isBoolPropertyWithSetAndExpression;

        public bool IsBoolProperty
        {
            get => this.isBoolProperty;
            set
            {
                this.isBoolProperty = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsBoolPropertyWithExpression
        {
            get => this.isBoolPropertyWithExpression;
            set
            {
                this.isBoolPropertyWithExpression = value;
                this.RaisePropertyChanged(() => this.IsBoolPropertyWithExpression);
            }
        }

        [SuppressMessage("Maintainability", "CA1507:Use nameof to express symbol names", Justification = "OK.")]
        public bool IsBoolPropertyWithSet
        {
            get => this.isBoolPropertyWithSet;
            set => _ = this.Set("IsBoolPropertyWithSet", ref isBoolPropertyWithSet, value);
        }

        public bool IsBoolPropertyWithSetAndExpression
        {
            get => this.isBoolPropertyWithSetAndExpression;
            set => _ = this.Set(() => IsBoolPropertyWithSetAndExpression, ref isBoolPropertyWithSetAndExpression, value);
        }
    }
}