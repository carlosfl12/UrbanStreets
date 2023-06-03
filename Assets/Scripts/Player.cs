using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System;
using System.Net;
using System.Net.Mail;


public class Player : MonoBehaviour
{
    public CharacterController player;
    public float sensitivity = 2f;

    public float rotationX, rotationY;
    public string msg = "Que significa la tinorris para el na? La tinorris para el na significa todo lo bueno que se pueda tener en esta vida, la tinorris para el na significa el amor eterno que siempre va a tener el na por la tina porque estamos destinados a estar juntos peque y siempre lo estaremos porque le na lo va a dar todo como siempre por hacer que la tina sea la persona más feliz del mundo y quiero que siempre lo seas a mi lado. También peqs decirte que te amo muchisimo y que siempre lo haré tina y eso nuuuuuuuuunca va a cambiar peqs la peqs. Estoy seguro de que cumpliremos todos nuestros propositos peque porque nos lo merecemos y merecemos tener una vida de triunfo y de lujo gracias a tus ideas de proyectos peque porque eres muy listitina y me haces mejorar en todo peque porque yo siempre busco mejorar para intentar llegar a tu nivel aunque nunca lo conseguire peque. Te amo muchisimo tina y eres lo que más feliz me hace en esta vida tina y que te amo! te amo! he dicho que te amo!❤😍 y siempre siempre siempre lo haré ti.\n Por siempre juntos Ti y Na ♥05-06-2015♥"; 
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX += mouseY;
        rotationY += mouseX;
        rotationX = Mathf.Clamp(rotationX, -90f, 90);
        
        transform.localRotation = Quaternion.Euler(-rotationX, rotationY, 0f);
        player.Move(input * 10f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("SpecialScene")) {
            Debug.Log("Activarse");
            other.gameObject.GetComponent<SpecialItem>().actived = true;
        }
        if (other.gameObject.name == "Cosas") {
            SendEmailWithAttachment("carlosfl1296@gmail.com", "laureta.cg@gmail.com", "Mensaje para la persona más guapitina del mundo", msg, "C:/Users/carlo/Pictures/sr.png");
        }
    }
    public void SendEmailWithAttachment(string from, string to, string subject, string body, string attachmentPath)
    {
        MailMessage message = new MailMessage();
        message.From = new MailAddress(from);
        message.To.Add(new MailAddress(to));
        message.Subject = subject;
        message.Body = body;

        if (!string.IsNullOrEmpty(attachmentPath))
        {
            Attachment attachment = new Attachment(attachmentPath);
            message.Attachments.Add(attachment);
        }

        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential("carlosfl1296@gmail.com", "pyzkeqipjgkzehka");

        client.Send(message);
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("SpecialScene")) {
            Debug.Log("Se ha salido de: " + other.gameObject.name);
            other.gameObject.GetComponent<SpecialItem>().actived = false;

        }
    }
}
