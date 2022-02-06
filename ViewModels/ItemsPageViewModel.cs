﻿// MIT License
// Copyright Eraware

using Eraware.Modules.SummitApiDemo.Data.Entities;
using System.Collections.Generic;

namespace Eraware.Modules.SummitApiDemo.ViewModels
{
    /// <summary>
    /// Represents a page of items, <see cref="Item"/>.
    /// </summary>
    public class ItemsPageViewModel
    {
        /// <summary>
        /// Gets or sets a list of items for this page.
        /// </summary>
        public List<ItemViewModel> Items { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the total amount of results found.
        /// </summary>
        public int ResultCount { get; set; }

        /// <summary>
        /// Gets or sets the total amount of pages available.
        /// </summary>
        public int PageCount { get; set; }
    }
}
