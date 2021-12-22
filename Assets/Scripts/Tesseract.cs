using UnityEngine;

public class Tesseract : MonoBehaviour
{
    [SerializeField] float rot_speed;
    float theta = 0;
    float[][] coordinates = new float[16][];
    float[][] final_coordinates = new float[16][];
    [SerializeField] GameObject[] spheres;
    [SerializeField] public Material line_mat;
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