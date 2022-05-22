
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

public interface ITransitionablePage {
    void TransitionTo();
    Page GetPage();
}
