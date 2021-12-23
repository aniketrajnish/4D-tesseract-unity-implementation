using UnityEngine;

public class Tesseract : MonoBehaviour
{
    [SerializeField] float rot_speed;
    [SerializeField] GameObject[] spheres;
    [SerializeField] public Material line_mat;
    [SerializeField] GameObject mesh_generator;

    float theta = 0;
    float[][] coordinates = new float[16][];
    float[][] final_coordinates = new float[16][];

    Mesh mesh;
    int[] triangles;
    Vector3[] vertices;
    private void Start()
    {
        coordinates[0] = new float[] { -1, -1, -1, 1 };
        coordinates[1] = new float[] { 1, -1, -1, 1 };
        coordinates[2] = new float[] { 1, 1, -1, 1 };
        coordinates[3] = new float[] { -1, 1, -1, 1 };
        coordinates[4] = new float[] { -1, -1, 1, 1 };
        coordinates[5] = new float[] { 1, -1, 1, 1 };
        coordinates[6] = new float[] { 1, 1, 1, 1 };
        coordinates[7] = new float[] { -1, 1, 1, 1 };
        coordinates[8] = new float[] { -1, -1, -1, -1 };
        coordinates[9] = new float[] { 1, -1, -1, -1 };
        coordinates[10] = new float[] { 1, 1, -1, -1 };
        coordinates[11] = new float[] { -1, 1, -1, -1 };
        coordinates[12] = new float[] { -1, -1, 1, -1 };
        coordinates[13] = new float[] { 1, -1, 1, -1 };
        coordinates[14] = new float[] { 1, 1, 1, -1 };
        coordinates[15] = new float[] { -1, 1, 1, -1 };

        mesh = new Mesh();
        mesh_generator.GetComponent<MeshFilter>().mesh = mesh;
    }

    float cam_dist = 2;
    int side_length = 5;

    private void Update()
    {
        theta += rot_speed * Time.deltaTime;

        float[][] rotation_matrix = new float[4][];
        rotation_matrix[0] = new float[] { 1, 0, 0, 0 };
        rotation_matrix[1] = new float[] { 0, 1, 0, 0 };
        rotation_matrix[2] = new float[] { 0, 0, Mathf.Cos(theta), -Mathf.Sin(theta) };
        rotation_matrix[3] = new float[] { 0, 0, Mathf.Sin(theta), Mathf.Cos(theta) };


        for (int i = 0; i < coordinates.Length; i++)
        {
            float[][] temp_coordinate_matrix = new float[4][];

            for (int k = 0; k < temp_coordinate_matrix.Length; k++)
            {
                temp_coordinate_matrix[k] = new float[] { coordinates[i][k] };
            }

            float[] rot_temp = matrix_multiplication_4d(rotation_matrix, temp_coordinate_matrix);

            float perspective_dist = 1 / (cam_dist - rot_temp[rot_temp.Length - 1]);

            float[] proj_temp = new float[rot_temp.Length];

            proj_temp[0] = perspective_dist * rot_temp[0] * side_length / 2;
            proj_temp[1] = perspective_dist * rot_temp[1] * side_length / 2;
            proj_temp[2] = perspective_dist * rot_temp[2] * side_length / 2;
            proj_temp[3] = perspective_dist * rot_temp[3] * side_length / 2;

            final_coordinates[i] = proj_temp;

            spheres[i].transform.position = new Vector3(proj_temp[0], proj_temp[1], proj_temp[2]);
        }

        /*for (int i = 0; i < 16; i += 4)
        {
            Vector3 coor_a = new Vector3(final_coordinates[i][0], final_coordinates[i][1], final_coordinates[i][2]);
            Vector3 coor_b = new Vector3(final_coordinates[i+1][0], final_coordinates[i+1][1], final_coordinates[i+1][2]);
            Vector3 coor_c = new Vector3(final_coordinates[i+2][0], final_coordinates[i+2][1], final_coordinates[i+2][2]);
            Vector3 coor_d = new Vector3(final_coordinates[i+3][0], final_coordinates[i+3][1], final_coordinates[i+3][2]);
            //DrawFaces(coor_a, coor_b, coor_c, coor_d);
        }*/
        DrawFaces();
    }

    void DrawEdge(int offset, int i, int j)
    {
        GL.Begin(GL.LINES);
        line_mat.SetPass(0);
        GL.Color(new Color(line_mat.color.r, line_mat.color.g, line_mat.color.b, line_mat.color.a));

        Vector3 start = new Vector3(final_coordinates[i + offset][0], final_coordinates[i + offset][1], final_coordinates[i + offset][2]);
        Vector3 end = new Vector3(final_coordinates[j + offset][0], final_coordinates[j + offset][1], final_coordinates[j + offset][2]);

        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);

        //Debug.DrawLine(start, end, Color.red);
    }

    private void OnPostRender()
    {
        for (int i = 0; i < 8; i++)
        {
            if (i < 4)
            {
                DrawEdge(0, i, i + 4);
                DrawEdge(0, i, i + 8);
                DrawEdge(0, i, (i + 1) % 4);
                DrawEdge(0, i + 4, ((i + 1) % 4) + 4);

                DrawEdge(8, i, i + 4);
                DrawEdge(8, i, (i + 1) % 4);
                DrawEdge(8, i + 4, ((i + 1) % 4) + 4);
            }

            DrawEdge(0, i, i + 8);
            GL.End();
        }

    }

    void DrawFaces()
    {
        vertices = new Vector3[16];

        for (int i = 0; i < 16; i++)
        {
            Vector3 coor_i = new Vector3(final_coordinates[i][0], final_coordinates[i][1], final_coordinates[i][2]);
            vertices[i] = coor_i;
        }

        /*triangles = new int[78];

        int j = 0;
        for (int i = 0; i < 13; i++)
        {
            triangles[j] = i;
            triangles[j + 1] = i + 1;
            triangles[j + 2] = i + 2;
            triangles[j + 3] = i + 2;
            triangles[j + 4] = i + 3;
            triangles[j + 5] = i;

            j += 6;
        }*/

        triangles = new int[] 
        { 
            6, 14, 13,
            13, 5, 6,
            6, 2, 1,
            1, 5, 6,
            6, 7, 4,
            4, 5, 6,
            6, 2, 10,
            10, 14, 6,
            6, 14, 15,
            15, 7, 6,
            6, 2, 3,
            3, 7, 6,
            14, 10, 13,
            13, 9, 14,
            14, 15, 12, 
            12, 13, 14,
            14, 15, 11,
            11, 10, 14,
            2, 10, 9,
            1, 2, 10,
            11, 3, 2,
            2, 10, 11,
            2, 3, 0,
            0, 1, 2,
            10, 11, 8,
            8, 9, 10,
            5, 4, 12,
            12, 13, 5,
            5, 1, 0,
            0, 4, 5,
            5, 1, 9,
            9, 13, 5,
            13, 9, 8,
            8, 12, 13,
            9, 1, 0,
            0, 8, 9,
            7, 3, 0,
            0, 4, 7,
            7, 15, 12,
            12, 4, 7,
            15, 7, 3, 
            3, 11, 15,
            15, 11, 8,
            8, 12, 15,
            3, 11, 8,
            8, 5, 3
        };

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    float[] matrix_multiplication_4d(float[][] mat_1, float[][] mat_2)
    {
        float[] result = new float[4];
        float[][] temp_matrix = new float[4][];

        temp_matrix[0] = new float[] { 0, 0, 0, 0 };
        temp_matrix[1] = new float[] { 0, 0, 0, 0 };
        temp_matrix[2] = new float[] { 0, 0, 0, 0 };
        temp_matrix[3] = new float[] { 0, 0, 0, 0 };

        if (mat_1[0].Length == mat_2.Length)
        {
            for (int i = 0; i < mat_1.Length; i++)
            {
                for (int j = 0; j < mat_2[0].Length; j++)
                {
                    float temp = 0;

                    for (int k = 0; k < mat_1[0].Length; k++)
                    {
                        temp += mat_1[i][k] * mat_2[k][j];
                    }
                    temp_matrix[i][j] = temp;
                }
            }
        }

        result[0] = temp_matrix[0][0];
        result[1] = temp_matrix[1][0];
        result[2] = temp_matrix[2][0];
        result[3] = temp_matrix[3][0];

        return result;
    }

}