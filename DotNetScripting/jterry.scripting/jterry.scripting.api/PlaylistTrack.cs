using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jterry.scripting.api
{
    public class PlaylistTrack
    {
        public virtual long Id { get; set; }
        public virtual Playlist Playlist { get; set; }
        public virtual Track Track { get; set; }
    }
}
