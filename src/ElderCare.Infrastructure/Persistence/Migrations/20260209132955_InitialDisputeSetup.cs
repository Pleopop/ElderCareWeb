using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElderCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDisputeSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisputeEvidences_Disputes_DisputeId",
                table: "DisputeEvidences");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Bookings_BookingId",
                table: "Disputes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DisputeEvidences",
                table: "DisputeEvidences");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "ResolvedBy",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "EvidenceUrl",
                table: "DisputeEvidences");

            migrationBuilder.RenameTable(
                name: "DisputeEvidences",
                newName: "DisputeEvidence");

            migrationBuilder.RenameColumn(
                name: "RaisedByUserId",
                table: "Disputes",
                newName: "RespondentId");

            migrationBuilder.RenameIndex(
                name: "IX_DisputeEvidences_DisputeId",
                table: "DisputeEvidence",
                newName: "IX_DisputeEvidence_DisputeId");

            migrationBuilder.AddColumn<decimal>(
                name: "ApprovedAmount",
                table: "Disputes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisputeType",
                table: "Disputes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FiledAt",
                table: "Disputes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "InitiatedBy",
                table: "Disputes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Disputes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedAmount",
                table: "Disputes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNotes",
                table: "Disputes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Disputes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedBy",
                table: "Disputes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Disputes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "EvidenceType",
                table: "DisputeEvidence",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "DisputeEvidence",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "DisputeEvidence",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "DisputeEvidence",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "DisputeEvidence",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "DisputeEvidence",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "DisputeEvidence",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UploadedBy",
                table: "DisputeEvidence",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_DisputeEvidence",
                table: "DisputeEvidence",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DisputeMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisputeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsAdminMessage = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisputeMessages_Disputes_DisputeId",
                        column: x => x.DisputeId,
                        principalTable: "Disputes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisputeMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_FiledAt",
                table: "Disputes",
                column: "FiledAt");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_InitiatedBy",
                table: "Disputes",
                column: "InitiatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_Priority",
                table: "Disputes",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_RespondentId",
                table: "Disputes",
                column: "RespondentId");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_ReviewedBy",
                table: "Disputes",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeEvidence_UploadedAt",
                table: "DisputeEvidence",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeEvidence_UploadedBy",
                table: "DisputeEvidence",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeMessages_DisputeId",
                table: "DisputeMessages",
                column: "DisputeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeMessages_SenderId",
                table: "DisputeMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_DisputeMessages_SentAt",
                table: "DisputeMessages",
                column: "SentAt");

            migrationBuilder.AddForeignKey(
                name: "FK_DisputeEvidence_Disputes_DisputeId",
                table: "DisputeEvidence",
                column: "DisputeId",
                principalTable: "Disputes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DisputeEvidence_Users_UploadedBy",
                table: "DisputeEvidence",
                column: "UploadedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Bookings_BookingId",
                table: "Disputes",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Users_InitiatedBy",
                table: "Disputes",
                column: "InitiatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Users_RespondentId",
                table: "Disputes",
                column: "RespondentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Users_ReviewedBy",
                table: "Disputes",
                column: "ReviewedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisputeEvidence_Disputes_DisputeId",
                table: "DisputeEvidence");

            migrationBuilder.DropForeignKey(
                name: "FK_DisputeEvidence_Users_UploadedBy",
                table: "DisputeEvidence");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Bookings_BookingId",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Users_InitiatedBy",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Users_RespondentId",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Users_ReviewedBy",
                table: "Disputes");

            migrationBuilder.DropTable(
                name: "DisputeMessages");

            migrationBuilder.DropIndex(
                name: "IX_Disputes_FiledAt",
                table: "Disputes");

            migrationBuilder.DropIndex(
                name: "IX_Disputes_InitiatedBy",
                table: "Disputes");

            migrationBuilder.DropIndex(
                name: "IX_Disputes_Priority",
                table: "Disputes");

            migrationBuilder.DropIndex(
                name: "IX_Disputes_RespondentId",
                table: "Disputes");

            migrationBuilder.DropIndex(
                name: "IX_Disputes_ReviewedBy",
                table: "Disputes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DisputeEvidence",
                table: "DisputeEvidence");

            migrationBuilder.DropIndex(
                name: "IX_DisputeEvidence_UploadedAt",
                table: "DisputeEvidence");

            migrationBuilder.DropIndex(
                name: "IX_DisputeEvidence_UploadedBy",
                table: "DisputeEvidence");

            migrationBuilder.DropColumn(
                name: "ApprovedAmount",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "DisputeType",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "FiledAt",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "InitiatedBy",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "RequestedAmount",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "ResolutionNotes",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Disputes");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "DisputeEvidence");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "DisputeEvidence");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "DisputeEvidence");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "DisputeEvidence");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "DisputeEvidence");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "DisputeEvidence");

            migrationBuilder.RenameTable(
                name: "DisputeEvidence",
                newName: "DisputeEvidences");

            migrationBuilder.RenameColumn(
                name: "RespondentId",
                table: "Disputes",
                newName: "RaisedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_DisputeEvidence_DisputeId",
                table: "DisputeEvidences",
                newName: "IX_DisputeEvidences_DisputeId");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Disputes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResolvedBy",
                table: "Disputes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EvidenceType",
                table: "DisputeEvidences",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "DisputeEvidences",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenceUrl",
                table: "DisputeEvidences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DisputeEvidences",
                table: "DisputeEvidences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DisputeEvidences_Disputes_DisputeId",
                table: "DisputeEvidences",
                column: "DisputeId",
                principalTable: "Disputes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Bookings_BookingId",
                table: "Disputes",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
