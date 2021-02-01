using System;
using JackboxGPT3.Games.Common.Models;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable EventNeverSubscribedTo.Global

namespace JackboxGPT3.Games.Common
{
    public interface IJackboxClient
    {
        public void Connect();
        public event EventHandler<ClientWelcome> PlayerStateChanged;
    }
}