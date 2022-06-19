using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataMapper.SimulationKernel.Migrations
{
    public partial class ExtendSimulationMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataLocation",
                table: "SimulationDataItems",
                newName: "OutputDataLocation");

            migrationBuilder.AddColumn<string>(
                name: "InputDataLocation",
                table: "SimulationDataItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputDataLocation",
                table: "SimulationDataItems");

            migrationBuilder.RenameColumn(
                name: "OutputDataLocation",
                table: "SimulationDataItems",
                newName: "DataLocation");
        }
    }
}
