using System;
using System.Windows.Forms;

namespace FluentDragDrop
{
	public class DelayedDragDefinition : DragDefinition
	{
		public DelayedDragDefinition(Control control, DragDropEffects allowedEffects)
			: base(control, allowedEffects)
		{
			ConditionEvaluator = () => true;
		}

		public DelayedDragDefinition If(Func<bool> conditionEvaluator)
		{
			ConditionEvaluator = conditionEvaluator;
			return this;
		}

		public DragOperation<T> WithData<T>(Func<T> dataEvaluator)
		{
			return new DragOperation<T>(this, dataEvaluator, ConditionEvaluator);
		}

		public Func<bool> ConditionEvaluator { get; private set; }
	}
}