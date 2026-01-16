export const Roles = {
  Admin: 'Admin',
  Instructor: 'Instructor',
  Client: 'Client'
} as const;

export type Role = typeof Roles[keyof typeof Roles];
