// MIT License
// Copyright Eraware
using Eraware.Modules.SummitApiDemo.ViewModels;

namespace Eraware.Modules.SummitApiDemo.Services
{
    /// <summary>
    /// Provides strongly typed localization services for this module.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets viewmodel that strongly types all resource keys.
        /// </summary>
        LocalizationViewModel ViewModel { get; }
    }
}