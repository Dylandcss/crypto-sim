import RegisterForm from '../../components/RegisterForm/RegisterForm'
import './RegisterPage.Module.css'

function RegisterPage() {
  return (
    <div className="register-page">
      <div className="register-card">
        <h1>Register</h1>
        <RegisterForm />
      </div>
    </div>
  )
}

export default RegisterPage
