using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElderCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFraudDetectionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchingResults_Beneficiaries_BeneficiaryId",
                table: "MatchingResults");

            migrationBuilder.CreateTable(
                name: "FraudAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InvestigatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InvestigatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraudAlerts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FraudScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    GPSScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    BookingScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    PaymentScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IdentityScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraudScores_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuspiciousActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RiskScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuspiciousActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuspiciousActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_AlertType",
                table: "FraudAlerts",
                column: "AlertType");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_DetectedAt",
                table: "FraudAlerts",
                column: "DetectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_Severity",
                table: "FraudAlerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_Status",
                table: "FraudAlerts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_UserId",
                table: "FraudAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudScores_CalculatedAt",
                table: "FraudScores",
                column: "CalculatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FraudScores_OverallScore",
                table: "FraudScores",
                column: "OverallScore");

            migrationBuilder.CreateIndex(
                name: "IX_FraudScores_UserId",
                table: "FraudScores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SuspiciousActivities_ActivityType",
                table: "SuspiciousActivities",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_SuspiciousActivities_DetectedAt",
                table: "SuspiciousActivities",
                column: "DetectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SuspiciousActivities_RiskScore",
                table: "SuspiciousActivities",
                column: "RiskScore");

            migrationBuilder.CreateIndex(
                name: "IX_SuspiciousActivities_UserId",
                table: "SuspiciousActivities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchingResults_Beneficiaries_BeneficiaryId",
                table: "MatchingResults",
                column: "BeneficiaryId",
                principalTable: "Beneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchingResults_Beneficiaries_BeneficiaryId",
                table: "MatchingResults");

            migrationBuilder.DropTable(
                name: "FraudAlerts");

            migrationBuilder.DropTable(
                name: "FraudScores");

            migrationBuilder.DropTable(
                name: "SuspiciousActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchingResults_Beneficiaries_BeneficiaryId",
                table: "MatchingResults",
                column: "BeneficiaryId",
                principalTable: "Beneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
