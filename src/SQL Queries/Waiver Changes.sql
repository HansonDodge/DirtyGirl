alter table Registration add Signature varchar(200)

alter table registration add IsIAmTheParticipant bit

alter table registration add IsSignatureConsent bit

update registration set IsSignatureConsent = 1, IsIAmTheParticipant = 1,Signature  = FirstName + ' ' + LastName

alter table registration  alter column IsIAmTheParticipant bit not null

alter table registration  alter column IsSignatureConsent bit not null
