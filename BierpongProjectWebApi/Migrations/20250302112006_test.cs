using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BierpongProjectWebApi.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_UserProfile_UserProfileUserId",
                table: "Friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendship",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_UserProfileUserId",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "UserProfileUserId",
                table: "Friendship");

            migrationBuilder.RenameTable(
                name: "Friendship",
                newName: "Friendships");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserProfile",
                newName: "user_id");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships",
                column: "FriendshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendUserId",
                table: "Friendships",
                column: "FriendUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId1",
                table: "Friendships",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_UserProfile_FriendUserId",
                table: "Friendships",
                column: "FriendUserId",
                principalTable: "UserProfile",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_UserProfile_UserId",
                table: "Friendships",
                column: "UserId",
                principalTable: "UserProfile",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_UserId1",
                table: "Friendships",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_User_user_id",
                table: "UserProfile",
                column: "user_id",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_UserProfile_FriendUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_UserProfile_UserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_UserId1",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_User_user_id",
                table: "UserProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_FriendUserId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId1",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Friendships");

            migrationBuilder.RenameTable(
                name: "Friendships",
                newName: "Friendship");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserProfile",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileUserId",
                table: "Friendship",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendship",
                table: "Friendship",
                column: "FriendshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserProfileUserId",
                table: "Friendship",
                column: "UserProfileUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_UserProfile_UserProfileUserId",
                table: "Friendship",
                column: "UserProfileUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId");
        }
    }
}
