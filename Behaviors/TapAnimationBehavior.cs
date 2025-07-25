using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp.Behaviors
{
    public class TapAnimationBehavior : Behavior<Frame>
    {
        protected override void OnAttachedTo(Frame bindable)
        {
            base.OnAttachedTo(bindable);
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                var f = (Frame)s;
                // Лёгкое уменьшение и возврат
                await f.ScaleTo(0.95, 50);
                await f.ScaleTo(1, 50);
            };
            bindable.GestureRecognizers.Add(tap);
        }
    }
}
