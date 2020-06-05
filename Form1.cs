using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestHttpClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        CommentService service  = new CommentService();
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var comments =await service.GetAllComments();

                // dump comments
                //comments.ForEach( c=> Debug.WriteLine($"{c.id } {c.name } {c.email } {c.body}"));

                var comment = await service.GetComment(1);
                MessageBox.Show($"{comments.Count} downloaded.");

            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            service.Cancel();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try { 
                var newComment = new Comment()
                {
                    body = "Test Body",
                    email = "email@wxample.com",
                    name = "name ",
                    postId = 1
                };
                var c = await service.PostComment(newComment);
                MessageBox.Show($"{c.id } {c.name} {c.email} {c.body}" );

            } catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
}
    }
}
