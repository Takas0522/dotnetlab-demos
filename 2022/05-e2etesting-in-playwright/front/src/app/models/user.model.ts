export interface UserModel {
  id: number;
  email: string;
  name: string;
  role: ROLE
}

const ROLE = {
  admin: 0,
  user: 1
} as const;
export type ROLE = typeof ROLE[keyof typeof ROLE];