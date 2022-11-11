import { JobTypeDTO } from '../../../global/models/job-type-dto';
import { GovernmentIdDTO } from './government-id-dto';

export interface UserForRegistrationDTO {
  firstName: string;
  LastName: string;
  email: string;
  jobType: JobTypeDTO;
  incomeLevel: number;
  governmentIdType: string;
  governmentId: GovernmentIdDTO;
  password: string;
  confirmPassword: string;
}
