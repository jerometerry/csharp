using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jterry.scripting.api
{
    public class Playlist
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }

        public virtual ICollection<PlaylistTrack> Tracks { get; set; }
    }
}
