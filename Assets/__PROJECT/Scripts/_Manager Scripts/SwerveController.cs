using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swerve
{
    public static class EnumHolder
    {
        public enum Axis
        {
            x,
            y,
            z,
        };

        public static Axis axis;

    }

    public static class SwerveController
    {
        [Header("Positions Of Mouse Gestures")]
        private static Vector3 firstMousePos, lastMousePos;

        [Header("Clamps")]
        [Range(0, 10)]
        [SerializeField] private static float clampOnAxis;
        [Range(0, 60)]
        [SerializeField] private static float rotationClamp;
        [Range(0,20)]
        [SerializeField] private static float rotationDelay;

        [Range(0, .1f)]
        [SerializeField] private static float sensForMove;
        [Range(0, 100)]
        [SerializeField] private static float forwardSpeed;

        [Header("Delays")]
        [SerializeField] private static float delayOfZeroRotation;

        [Header("Booleans")]
        [SerializeField] private static bool checkForOnlyInput;
        [SerializeField] private static bool checkForBoolLevelParams;


        private static Vector3 direction;
        private static Vector3 movementDirection;


        public static Vector3 differVec;
        private static Vector3 rotVec;
        private static Vector3 simpleVector;
        /// <summary>
        /// Set object speed on direction of Vector3.forward
        /// </summary>
        public static void MoveOnLine(Transform Transform, float speed)
        {
            Transform.position += movementDirection * speed * Time.deltaTime; //Objeyi ilerlettiğimiz kısım
        }
        /// <summary>
        /// Set object transform, clamp value on the X Axis, sensivity on the X Axis, checker for only Input
        /// </summary>
        /// <retur>
        public static void MoveAndRotateOnAxis(
            Transform Transform,
            float clampOnAxis,
            bool doRotation = false,
            bool checkForClamp = false,
            bool checkForOnlyInput = false,
            EnumHolder.Axis axis = EnumHolder.Axis.x,
            float sensForMove = .15f,
            float rotationClamp = 30,
            float rotationDelay = 5)
        {

            if (checkForOnlyInput) //Ekrana birden fazla parmak dokununca hata almamak adına if kontrolü.
                if (Input.touchCount != 1)
                    return;

            if (Input.GetMouseButtonDown(0)) //İlk dokunduğumuz noktanın pozisyonunu almak için if kontrolü
                firstMousePos = Input.mousePosition;

            if (Input.GetMouseButton(0)) //İlk dokunmadan sonra sürekli pozisyon bilgisi almak için if kontrolü
            {
                lastMousePos = Input.mousePosition; //Aldığımız sürekli pozisyon değeri (devamlı olarak dokunduğumuz pozisyonla kendini yeniliyor)

                differVec = lastMousePos - firstMousePos; //İlk ve sonraki input pozisyonlarının farkını alıyoruz

                simpleVector = new Vector3(differVec.x * direction.x, differVec.y * direction.y, differVec.z * direction.z) * .8f; //Switch-Case'den aldığımız bilgiye göre hareket edeceğimiz pozisyonu belirliyoruz

                differVec = Transform.localPosition + (simpleVector * sensForMove * Time.deltaTime); //Çeşitli hassaslık ayarları. -->Smooth'luğu sağlamak için

                Transform.localPosition = differVec; //Hassaslık ayarlarından sonra elde ettiğimiz veriyi Objemizin pozisyonuna eşitliyoruz.

                switch (axis) //Swerve yapacağımız ekseni ve ilerleyeceğimiz ekseni belirlediğimiz kısım
                {
                    case EnumHolder.Axis.x:
                        movementDirection = Vector3.forward;
                        direction = Vector3.right;
                        rotVec = Vector3.up;
                        if (checkForClamp)
                            Transform.localPosition = new Vector3(Mathf.Clamp(Transform.localPosition.x, -clampOnAxis, clampOnAxis), Transform.localPosition.y, Transform.localPosition.z);
                        break;
                    case EnumHolder.Axis.y:
                        movementDirection = Vector3.forward;
                        direction = Vector3.right;
                        rotVec = Vector3.up;
                        if (checkForClamp)
                            Transform.localPosition = new Vector3(Mathf.Clamp(Transform.localPosition.x, -clampOnAxis, clampOnAxis), Transform.localPosition.y, Transform.localPosition.z);
                        break;
                    case EnumHolder.Axis.z:
                        movementDirection = Vector3.forward;
                        direction = Vector3.right;
                        rotVec = Vector3.forward;
                        if (checkForClamp)
                            Transform.localPosition = new Vector3(Mathf.Clamp(Transform.localPosition.x, -clampOnAxis, clampOnAxis), Transform.localPosition.y, Transform.localPosition.z);
                        break;
                }

                if (doRotation) //Obje swerve yaparken rotasyonunu da etkilensin istersek buradaki if kontrolüne giriyoruz
                {
                    if (simpleVector.x > 0 || simpleVector.y > 0 || simpleVector.z > 0)
                        Transform.localRotation = Quaternion.Slerp(Transform.localRotation, Quaternion.Euler(rotVec * rotationClamp), rotationDelay * Time.deltaTime);
                    else if (simpleVector.x < 0 || simpleVector.y < 0 || simpleVector.z < 0)
                        Transform.localRotation = Quaternion.Slerp(Transform.localRotation, Quaternion.Euler(rotVec * -rotationClamp), rotationDelay * Time.deltaTime);
                    else
                        Transform.localRotation = Quaternion.Slerp(Transform.localRotation, Quaternion.Euler(0, 0, 0), rotationDelay * Time.deltaTime);
                }

                firstMousePos = Input.mousePosition; //İlk ve son inputtaki pozisyon farklılığını yeniliyoruz ki parmağımızı soldan sağa getirip sabit tutunca hala elimizi çekmezsek hareket etmeyelim. (Joystick gibi davranmamak için)
            }
            else if(doRotation)
                Transform.localRotation = Quaternion.Slerp(Transform.localRotation, Quaternion.Euler(0, 0, 0), rotationDelay * Time.deltaTime); // İnput olmadığında rotasyonumuzu sıfırlıyoruz

        }
    }
}