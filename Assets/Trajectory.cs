using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    //Skrip, collider, dan rigidbody bola
    public BallControl ball;
    CircleCollider2D ballCollider;
    Rigidbody2D ballRigidbody;

    //Bola "bayangan" yang akan ditampilkan di titik tumbukan
    public GameObject ballAtCollision;

    // Start is called before the first frame update
    void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
        ballCollider = ball.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Inisiasi status pantulan lintasan, yang hanya akan ditampilkan jika lintasan bertumbukan dengan objek tertentu
        bool drawBallAtCollision = false;

        // Titik tumbukan yang digeser, untuk menggambarkan ballAtCollision
        Vector2 offsetHitPoint = new Vector2();

        //Tentukan titik tumbukan dengan deteksi pergerakan lingkaran
        RaycastHit2D[] circleCastHit2DArray = Physics2D.CircleCastAll(ballRigidbody.position, ballCollider.radius,
            ballRigidbody.velocity.normalized);

        // Untuk setiap titik tumbukan
        foreach(RaycastHit2D circleCasHit2D in circleCastHit2DArray)
        {
            //Jika terjadi tumbuan, dan tumbukan tersebut tidak dengan bola
            //karena garis lintasan digambar dari titik tengah bola
            if(circleCasHit2D.collider != null &&
                circleCasHit2D.collider.GetComponent<BallControl>() == null)
            {
                //Garis lintasan akan digambar dari titik tengah bola saat ini ke titik tengah bola pada saat tumbukan
                //yaitu sebuah titi yang di offset dari titik tumbukan berdasar vektor normal titik tersebut sebesar
                //jari jari bola

                //Tentukan titik tumbukan
                Vector2 hitPoint = circleCasHit2D.point;

                //Tentukan normal di titik tumbukan 
                Vector2 hitNormal = circleCasHit2D.normal;

                //Tentukan offsetHotPoint, yaitu titik tengah bola pada saat bertumbukan
                offsetHitPoint = hitPoint + hitNormal * ballCollider.radius;

                // Gambar garis lintasan dari titik tengah bola saat ini ke titik tenga bola pada saat bertumbukan
                DottedLine.DottedLine.Instance.DrawDottedLine(ball.transform.position, offsetHitPoint);

                // Kalau bukan side wall, gambar pantulannya
                if (circleCasHit2D.collider.GetComponent<SideWall>() == null)
                {
                    //Hitung vektir datang
                    Vector2 inVector = (offsetHitPoint - ball.TrajectoriOrigin).normalized;

                    //Hitung vector keluar
                    Vector2 outVector = Vector2.Reflect(inVector, hitNormal);

                    //Hitung dot product dari outVector dan hitNormal. DIgunakan supaya garus lintasan ketika
                    // terjaditubukan tidak digambar
                    float outDot = Vector2.Dot(outVector, hitNormal);

                    if(outDot > -1.0f && outDot < 1.0)
                    {
                        //Gambar lintasan pantulannya
                        DottedLine.DottedLine.Instance.DrawDottedLine(
                            offsetHitPoint,
                            offsetHitPoint + outVector * 10.0f);

                        //Untuk menggambarkan bola "bayangan" diprediksi titik tumbukan
                        drawBallAtCollision = true;
                    }
                }

                //Hanya gambar lintasan untuk satututuk tumbukan, jadi keluar dari loop
                break;
            }        
        }

        //JIka true
        if (drawBallAtCollision)
        {
            //Gambar bila "bayangan" diprediksi titik tumbukan
            ballAtCollision.transform.position = offsetHitPoint;

            ballAtCollision.SetActive(true);
        }
        else
        {
            //Sembunyikan bola "bayangan"
            ballAtCollision.SetActive(false);
        }
    }
}
