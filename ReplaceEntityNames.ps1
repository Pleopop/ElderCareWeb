# Batch replace CustomerProfile and CaregiverProfile in Application layer

$files = @(
    "src\ElderCare.Application\Services\MatchingService.cs",
    "src\ElderCare.Application\Features\Tracking\Commands\TrackingCommands.cs",
    "src\ElderCare.Application\Features\Reviews\Commands\ReviewCommands.cs",
    "src\ElderCare.Application\Features\Payments\Commands\PaymentCommands.cs",
    "src\ElderCare.Application\Features\Matching\Queries\MatchingQueries.cs",
    "src\ElderCare.Application\Features\Matching\Commands\MatchingCommands.cs",
    "src\ElderCare.Application\Features\Matching\DTOs\MatchingDTOs.cs",
    "src\ElderCare.Application\Features\Admin\DTOs\AdminDTOs.cs",
    "src\ElderCare.Application\Features\Admin\Commands\AdminCommands.cs",
    "src\ElderCare.Application\Features\Bookings\Queries\BookingQueries.cs",
    "src\ElderCare.Application\Features\Auth\Commands\AuthCommands.cs",
    "src\ElderCare.Application\Features\Bookings\DTOs\BookingDTOs.cs",
    "src\ElderCare.Application\Features\Bookings\Commands\BookingCommands.cs",
    "src\ElderCare.Application\Common\Interfaces\IServices.cs"
)

foreach ($file in $files) {
    $fullPath = Join-Path $PSScriptRoot $file
    if (Test-Path $fullPath) {
        Write-Host "Processing: $file"
        $content = Get-Content $fullPath -Raw
        
        # Replace CaregiverProfile with Caregiver
        $content = $content -replace 'CaregiverProfile', 'Caregiver'
        
        # Replace CustomerProfile with Customer
        $content = $content -replace 'CustomerProfile', 'Customer'
        
        # Replace FK property names
        $content = $content -replace 'CaregiverProfileId', 'CaregiverId'
        $content = $content -replace 'CustomerProfileId', 'CustomerId'
        
        Set-Content $fullPath -Value $content -NoNewline
        Write-Host "  ✓ Updated"
    } else {
        Write-Host "  ✗ File not found: $fullPath"
    }
}

Write-Host "`nDone! All files updated."
