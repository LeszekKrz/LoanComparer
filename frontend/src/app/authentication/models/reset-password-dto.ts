export interface ResetPasswordDTO {
  password: string;
  confirmPassword: string;
  email: string;
  token: string;
}
