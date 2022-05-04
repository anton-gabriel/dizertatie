namespace SimulationKernel.Data
{
  using DomainModel.SimulationKernel;
  public class ProcessedDataService
  {
    public async Task<ObjectData> ReadObjFileAsync(Stream stream)
    {
      (IList<float[]> vertices, IList<int[]> faces) = await ReadObjDataAsync(stream);
      // Convert vertices and faces to an array of triangles
      var triangles = new List<double[]>();
      for (int i = 0; i < faces.Count; i++)
      {
        var face = faces[i];
        var v1 = vertices[face[0]];
        var v2 = vertices[face[1]];
        var v3 = vertices[face[2]];
        triangles.Add(new double[] { v1[0], v1[1], v1[2] });
        triangles.Add(new double[] { v2[0], v2[1], v2[2] });
        triangles.Add(new double[] { v3[0], v3[1], v3[2] });
      }
      return new ObjectData(triangles.ToArray());
    }

    public static async Task<(IList<float[]>, IList<int[]>)> ReadObjDataAsync(Stream stream)
    {
      var vertices = new List<float[]>();
      var faces = new List<int[]>();

      using (var reader = new StreamReader(stream))
      {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
          if (line.StartsWith("v "))
          {
            var parts = line.Split(' ');
            vertices.Add(new float[] { float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]) });
          }
          else if (line.StartsWith("f "))
          {
            var parts = line.Split(' ');
            faces.Add(new int[] { int.Parse(parts[1]) - 1, int.Parse(parts[2]) - 1, int.Parse(parts[3]) - 1 });
          }
        }
      }
      return (vertices, faces);
    }

    public Task<double[][]> ReadObjFile(string objFilePath)
    {
      (IList<float[]> vertices, IList<int[]> faces) = ReadObjData(objFilePath);
      // Convert vertices and faces to an array of triangles
      var triangles = new List<double[]>();
      for (int i = 0; i < faces.Count; i++)
      {
        var face = faces[i];
        var v1 = vertices[face[0]];
        var v2 = vertices[face[1]];
        var v3 = vertices[face[2]];
        triangles.Add(new double[] { v1[0], v1[1], v1[2] });
        triangles.Add(new double[] { v2[0], v2[1], v2[2] });
        triangles.Add(new double[] { v3[0], v3[1], v3[2] });
      }
      return Task.FromResult(triangles.ToArray());
    }

    // Read the .obj file and return the vertices and faces
    public static (IList<float[]>, IList<int[]>) ReadObjData(string objFilePath)
    {
      var vertices = new List<float[]>();
      var faces = new List<int[]>();

      using (var reader = new StreamReader(objFilePath))
      {
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
          if (line.StartsWith("v "))
          {
            var parts = line.Split(' ');
            vertices.Add(new float[] { float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]) });
          }
          else if (line.StartsWith("f "))
          {
            var parts = line.Split(' ');
            faces.Add(new int[] { int.Parse(parts[1]) - 1, int.Parse(parts[2]) - 1, int.Parse(parts[3]) - 1 });
          }
        }
      }
      return (vertices, faces);
    }
  }
}