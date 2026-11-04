
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KnowledgeArchiveManager : MonoBehaviour
{
    [Header("Article Display")]
    public TMP_Text articleText;

    [Header("Topic Buttons")]
    public Button apiButton;
    public Button ramButton;
    public Button cloudButton;
    public Button queryButton;
    public Button binaryButton;
    public Button webButton;
    public Button loginButton;
    public Button virusButton;
    public Button pixelButton;
    public Button serverButton;

    private void Start()
    {
        apiButton.onClick.AddListener(() => ShowArticle("API"));
        ramButton.onClick.AddListener(() => ShowArticle("RAM"));
        cloudButton.onClick.AddListener(() => ShowArticle("CLOUD"));
        queryButton.onClick.AddListener(() => ShowArticle("QUERY"));
        binaryButton.onClick.AddListener(() => ShowArticle("BINARY"));
        webButton.onClick.AddListener(() => ShowArticle("WEB"));
        loginButton.onClick.AddListener(() => ShowArticle("LOGIN"));
        virusButton.onClick.AddListener(() => ShowArticle("VIRUS"));
        pixelButton.onClick.AddListener(() => ShowArticle("PIXEL"));
        serverButton.onClick.AddListener(() => ShowArticle("SERVER"));

        articleText.text =
            "Select a topic to learn more about its meaning, history, development, and real-world uses.";
    }

    public void ShowArticle(string topic)
    {
        switch (topic)
        {
            case "API":
                articleText.text =
                    "<b>API – Application Programming Interface</b>\n\n" +
                    "<b>Meaning:</b> An API is a set of rules that allows different software applications to communicate and exchange information.\n\n" +
                    "<b>History:</b> APIs developed alongside early operating systems and software libraries. Web APIs became widely used as internet services grew.\n\n" +
                    "<b>Development:</b> Many developers and organizations contributed to modern API standards. There is no single inventor of the API.\n\n" +
                    "<b>Evolution and Uses:</b> APIs evolved from basic software interfaces into web services used by apps, websites, payment systems, and online platforms.";
                break;

            case "RAM":
                articleText.text =
                    "<b>RAM – Random Access Memory</b>\n\n" +
                    "<b>Meaning:</b> RAM is temporary computer memory that stores data and instructions currently being used by the processor.\n\n" +
                    "<b>History:</b> Early forms of random-access memory appeared in the 1940s and 1950s. Semiconductor RAM became important in computers during the 1960s and 1970s.\n\n" +
                    "<b>Development:</b> Many researchers and companies contributed to RAM technology. Robert Dennard is known for inventing dynamic RAM (DRAM) in 1967.\n\n" +
                    "<b>Evolution and Uses:</b> RAM evolved from magnetic-core memory to modern DDR memory. It helps computers run programs and multitask.";
                break;

            case "CLOUD":
                articleText.text =
                    "<b>CLOUD – Cloud Computing</b>\n\n" +
                    "<b>Meaning:</b> Cloud computing provides computing services such as storage, servers, and software through the internet.\n\n" +
                    "<b>History:</b> The idea of computing as a utility dates back to the 1960s. Cloud services became widely available in the 2000s.\n\n" +
                    "<b>Development:</b> Many researchers and technology companies helped develop cloud computing. It has no single inventor.\n\n" +
                    "<b>Evolution and Uses:</b> Cloud computing now supports online storage, website hosting, collaboration tools, streaming, and business applications.";
                break;

            case "QUERY":
                articleText.text =
                    "<b>QUERY</b>\n\n" +
                    "<b>Meaning:</b> A query is a request for information or an instruction to retrieve or manipulate data, often in a database.\n\n" +
                    "<b>History:</b> Database queries became important as database management systems developed in the 1960s and 1970s.\n\n" +
                    "<b>Development:</b> SQL was developed at IBM by Donald Chamberlin and Raymond Boyce in the 1970s, based on earlier relational database research by Edgar F. Codd.\n\n" +
                    "<b>Evolution and Uses:</b> Queries are now used in databases, search engines, websites, and applications to find and manage information.";
                break;

            case "BINARY":
                articleText.text =
                    "<b>BINARY</b>\n\n" +
                    "<b>Meaning:</b> Binary is a number system that uses only two digits: 0 and 1. Computers use binary to represent and process data.\n\n" +
                    "<b>History:</b> The modern binary number system was described by Gottfried Wilhelm Leibniz in 1703. Earlier binary ideas existed in other cultures and systems.\n\n" +
                    "<b>Development:</b> George Boole's work on Boolean algebra also helped establish the logic used in digital computing.\n\n" +
                    "<b>Evolution and Uses:</b> Binary is fundamental to computer instructions, digital images, files, and electronic communication.";
                break;

            case "WEB":
                articleText.text =
                    "<b>WEB – World Wide Web</b>\n\n" +
                    "<b>Meaning:</b> The World Wide Web is a system of connected pages and resources accessed through the internet using web browsers.\n\n" +
                    "<b>History:</b> Tim Berners-Lee invented the World Wide Web while working at CERN. He proposed it in 1989 and developed its first website in 1990.\n\n" +
                    "<b>Development:</b> Berners-Lee created important early web technologies, including HTML, HTTP, and URLs.\n\n" +
                    "<b>Evolution and Uses:</b> The Web evolved from simple text pages into interactive websites, online services, social media, and web applications.";
                break;

            case "LOGIN":
                articleText.text =
                    "<b>LOGIN</b>\n\n" +
                    "<b>Meaning:</b> Login is the process of accessing a computer system or online account using credentials such as a username and password.\n\n" +
                    "<b>History:</b> Login systems became common with multi-user computer systems and time-sharing environments in the 1960s.\n\n" +
                    "<b>Development:</b> Fernando Corbató and his team at MIT helped establish password-based access for computer users through the Compatible Time-Sharing System.\n\n" +
                    "<b>Evolution and Uses:</b> Modern login systems may use passwords, biometrics, security keys, and multi-factor authentication.";
                break;

            case "VIRUS":
                articleText.text =
                    "<b>VIRUS – Computer Virus</b>\n\n" +
                    "<b>Meaning:</b> A computer virus is malicious software that can copy itself by attaching to other files or programs.\n\n" +
                    "<b>History:</b> Early self-replicating computer program concepts appeared in the 1970s. The Elk Cloner virus appeared in 1982 and affected Apple II computers.\n\n" +
                    "<b>Development:</b> Fred Cohen formally demonstrated and studied computer viruses in 1983. The term and research helped establish the field of computer virus security.\n\n" +
                    "<b>Evolution and Uses:</b> Computer viruses evolved alongside computer technology. They can damage files, disrupt systems, or spread malicious code.";
                break;

            case "PIXEL":
                articleText.text =
                    "<b>PIXEL – Picture Element</b>\n\n" +
                    "<b>Meaning:</b> A pixel is the smallest addressable element of a digital image or display. It represents a tiny part of the picture.\n\n" +
                    "<b>History:</b> The term pixel is commonly credited to Frederic C. Billingsley, who used it in the 1960s while working with digital image technology.\n\n" +
                    "<b>Development:</b> Pixel technology advanced through television, computer graphics, digital cameras, and display research.\n\n" +
                    "<b>Evolution and Uses:</b> Pixels are used in monitors, smartphones, photographs, digital art, and video games.";
                break;

            case "SERVER":
                articleText.text =
                    "<b>SERVER</b>\n\n" +
                    "<b>Meaning:</b> A server is a computer or software system that provides data, resources, or services to other computers called clients.\n\n" +
                    "<b>History:</b> Server computing developed from early mainframe and time-sharing systems. Client-server computing became widely used in the 1980s and 1990s.\n\n" +
                    "<b>Development:</b> Server technology was developed by many computer scientists and technology companies. It has no single inventor.\n\n" +
                    "<b>Evolution and Uses:</b> Modern servers power websites, databases, online games, file sharing, email, and cloud services.";
                break;
        }
    }
}
