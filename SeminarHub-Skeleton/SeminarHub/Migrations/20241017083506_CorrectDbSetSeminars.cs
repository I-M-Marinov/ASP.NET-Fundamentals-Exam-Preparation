using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeminarHub.Migrations
{
    /// <inheritdoc />
    public partial class CorrectDbSetSeminars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seminar_AspNetUsers_OrganizerId",
                table: "Seminar");

            migrationBuilder.DropForeignKey(
                name: "FK_Seminar_Categories_CategoryId",
                table: "Seminar");

            migrationBuilder.DropForeignKey(
                name: "FK_SeminarsParticipants_Seminar_SeminarId",
                table: "SeminarsParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seminar",
                table: "Seminar");

            migrationBuilder.RenameTable(
                name: "Seminar",
                newName: "Seminars");

            migrationBuilder.RenameIndex(
                name: "IX_Seminar_OrganizerId",
                table: "Seminars",
                newName: "IX_Seminars_OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_Seminar_CategoryId",
                table: "Seminars",
                newName: "IX_Seminars_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seminars",
                table: "Seminars",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Seminars_AspNetUsers_OrganizerId",
                table: "Seminars",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seminars_Categories_CategoryId",
                table: "Seminars",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeminarsParticipants_Seminars_SeminarId",
                table: "SeminarsParticipants",
                column: "SeminarId",
                principalTable: "Seminars",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seminars_AspNetUsers_OrganizerId",
                table: "Seminars");

            migrationBuilder.DropForeignKey(
                name: "FK_Seminars_Categories_CategoryId",
                table: "Seminars");

            migrationBuilder.DropForeignKey(
                name: "FK_SeminarsParticipants_Seminars_SeminarId",
                table: "SeminarsParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seminars",
                table: "Seminars");

            migrationBuilder.RenameTable(
                name: "Seminars",
                newName: "Seminar");

            migrationBuilder.RenameIndex(
                name: "IX_Seminars_OrganizerId",
                table: "Seminar",
                newName: "IX_Seminar_OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_Seminars_CategoryId",
                table: "Seminar",
                newName: "IX_Seminar_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seminar",
                table: "Seminar",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Seminar_AspNetUsers_OrganizerId",
                table: "Seminar",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seminar_Categories_CategoryId",
                table: "Seminar",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeminarsParticipants_Seminar_SeminarId",
                table: "SeminarsParticipants",
                column: "SeminarId",
                principalTable: "Seminar",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
