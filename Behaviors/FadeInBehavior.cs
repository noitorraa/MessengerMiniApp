using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerMiniApp.Behaviors
{
    internal class FadeInBehavior : Behavior<Frame>
    {
        protected override void OnAttachedTo(Frame bindable)
        {
            base.OnAttachedTo(bindable);

            // изначально скрываем
            bindable.Opacity = 0;

            // подписываемся на Loaded
            bindable.Loaded += OnFrameLoaded;
        }

        protected override void OnDetachingFrom(Frame bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Loaded -= OnFrameLoaded;
        }

        private async void OnFrameLoaded(object sender, EventArgs e)
        {
            if (sender is Frame frame)
            {
                // Убираем подписку, чтобы анимация сработала только один раз
                frame.Loaded -= OnFrameLoaded;

                // Плавно «проявляем» за 250 мс
                await frame.FadeTo(1, 250);
            }
        }
    }
}
