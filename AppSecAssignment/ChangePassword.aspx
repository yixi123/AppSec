<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="AppSecAssignment.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change Password</title>
    <script type="text/javascript">
        function validate() {
            var str = document.getElementById("<%=tb_newpwd.ClientID %>").value;

            if (str.length < 8) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password Length Must be at Least 8 Characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at Least 1 number";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at Least 1 Upper case";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_upper_case");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at Least 1 lower case";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_lower_case");
            }
            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password require at Least 1 special character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_special_character");
            }

            document.getElementById("lbl_pwdchecker").innerHTML = "Excellent";
            document.getElementById("lbl_pwdchecker").style.color = "Blue";


        }
    </script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.0.0-beta1/css/bootstrap.min.css" integrity="sha512-thoh2veB35ojlAhyYZC0eaztTAUhxLvSZlWrNtlV01njqs/UdY3421Jg7lX0Gq9SRdGVQeL8xeBp9x1IPyL1wQ==" crossorigin="anonymous" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.0.0-beta1/js/bootstrap.min.js" integrity="sha512-ZvbjbJnytX9Sa03/AcbP/nh9K95tar4R0IAxTS2gh2ChiInefr1r7EpnVClpTWUEN7VarvEsH3quvkY1h0dAFg==" crossorigin="anonymous"></script>
</head>
<body>
    
    <form id="form1" runat="server" defaultbutton="btn_submit">
        <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
      <div class="container-fluid">
        <a class="navbar-brand" href="#">SITConnect</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarSupportedContent">
          <ul class="navbar-nav me-auto mb-2 mb-lg-0">
            <li class="nav-item">
              <a class="nav-link active" href="#">Home</a>
            </li>
          </ul>
          <div class="d-flex">
              <asp:LinkButton ID="LinkButton1" runat="server" OnClick="btn_logout_Click">Logout</asp:LinkButton>
          </div>
        </div>
      </div>
    </nav>
        <div>
            <h2>Change Password</h2>
            <asp:Label ID="lblMsg" runat="server" style="margin:auto;"></asp:Label>
            <br />
            <table style="width:100%;">
                <tr>
                    <td>User ID</td>
                    <td>
                        <asp:Label ID="lblUserId" runat="server"></asp:Label>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td>Password</td>
                    <td>
                        <asp:TextBox ID="tb_pwd" runat="server" TextMode="Password"></asp:TextBox>
                        
                    </td>
                    <td>
                        <asp:Label ID="lblErrorMsgPwd" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:20%;">New Password</td>
                    <td style="width:20%;">
                        <asp:TextBox ID="tb_newpwd" runat="server" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
                        
                    </td>
                    <td><asp:Label ID="lbl_pwdchecker" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>Confirm Password</td>
                    <td>
                        <asp:TextBox ID="tb_cfmpwd" runat="server" TextMode="Password"></asp:TextBox>
                        
                    </td>
                    <td><asp:Label ID="lblErrorMsgCfmPwd" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td>
                        
                    </td>
                    <td>
                        <asp:Button ID="btn_back" runat="server" Text="Back" OnClick="btn_back_Click" CssClass="btn btn-secondary" />
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_submit" runat="server" Text="Confirm" OnClick="btn_submit_Click" CssClass="btn btn-primary" />
                    </td>
                    <td></td>
                </tr>
                </table>
                
        </div>
    </form>
</body>
</html>
