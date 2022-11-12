import { JobTypeDTO } from '../../core/models/job-type-dto';
import { GovernmentIdDTO } from './government-id-dto';

export interface UserForRegistrationDTO {
  firstName: string;
  lastName: string;
  email: string;
  jobType: JobTypeDTO;
  incomeLevel: number;
  governmentId: GovernmentIdDTO;
  password: string;
  confirmPassword: string;
}
