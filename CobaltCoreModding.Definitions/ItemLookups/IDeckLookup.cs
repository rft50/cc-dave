﻿using CobaltCoreModding.Definitions.ExternalItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CobaltCoreModding.Definitions.ItemLookups
{
    public interface IDeckLookup: ISpriteLookup, IManifestLookup, ICobaltCoreLookup
    {
        ExternalDeck LookupDeck(string globalName);
    }
}
