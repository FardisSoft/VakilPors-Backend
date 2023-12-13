using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services
{
    public class LegalDocumentService : ILegalDocumentService
    {
        private readonly IAppUnitOfWork _uow;
        private readonly IAwsFileService _fileService;
        private readonly ILawyerServices _lawyerServices;
        private readonly IMapper _mapper;
        private readonly IEmailSender emailSender;
        private readonly ITelegramService _tegramService;
        private readonly IPremiumService _premiumService;
        public LegalDocumentService(ILawyerServices lawyerServices, IAwsFileService fileService, IAppUnitOfWork uow, IMapper mapper, IEmailSender emailSender, ITelegramService telegramService, IPremiumService premiumService)
        {
            _lawyerServices = lawyerServices;
            _fileService = fileService;
            _uow = uow;
            _mapper = mapper;
            this.emailSender = emailSender;
            this._tegramService = telegramService;
            _premiumService = premiumService;
        }

        public async Task<LegalDocumentDto> AddDocument(int userId, LegalDocumentDto documentDto)
        {
            if (await _lawyerServices.IsLawyer(userId))
                throw new BadArgumentException("Lawyers can not have document");

            documentDto.UserId = userId;

            if (documentDto.File != null)
                documentDto.FileUrl = await _fileService.UploadAsync(documentDto.File);

            var document = _mapper.Map<LegalDocument>(documentDto);

            await _uow.DocumentRepo.AddAsync(document);

            var result = await _uow.SaveChangesAsync();
            if (result <= 0)
                throw new Exception();

            return _mapper.Map<LegalDocumentDto>(document);
        }

        public async Task<LegalDocumentDto> UpdateDocument(LegalDocumentDto documentDto)
        {
            var foundDoc = await _uow.DocumentRepo.FindAsync(documentDto.Id);

            if (foundDoc == null)
                throw new BadArgumentException("document not found");

            if (documentDto.File != null)
                documentDto.FileUrl = await _fileService.UploadAsync(documentDto.File);

            foundDoc.Description = documentDto.Description;
            foundDoc.Title = documentDto.Title;
            foundDoc.FileUrl = documentDto.FileUrl;
            foundDoc.DocumentCategory = documentDto.DocumentCategory;
            foundDoc.MinimumBudget = documentDto.MinimumBudget;
            foundDoc.MaximumBudget = documentDto.MaximumBudget;
            foundDoc.CaseName = documentDto.CaseName;

            _uow.DocumentRepo.Update(foundDoc);

            var result = await _uow.SaveChangesAsync();
            if (result <= 0)
                throw new Exception();

            return _mapper.Map<LegalDocumentDto>(foundDoc);

        }

        public async Task<bool> DeleteDocument(int documentId)
        {
            var foundDoc = await _uow.DocumentRepo.FindAsync(documentId);

            if (foundDoc == null)
                throw new BadArgumentException("document not found");

            _uow.DocumentRepo.Remove(foundDoc);

            var result = await _uow.SaveChangesAsync();
            if (result <= 0)
                throw new Exception();

            return true;
        }

        public async Task<LegalDocument> GetDocumentById(int documentId)
        {
            var doc = await _uow.DocumentRepo
                .AsQueryable()
                .Include(x=>x.Accesses)
                .ThenInclude(a=>a.Lawyer)
                .Where(x => x.Id == documentId)
                .FirstOrDefaultAsync();

            if (doc == null)
                throw new BadArgumentException("document not found");

            return doc;
        }

        public async Task<List<LegalDocument>> GetDocumentsByUserId(int userId,Status? status)
        {
            var docs = await _uow.DocumentRepo
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.Accesses)
                .ThenInclude(a => a.Lawyer)
                .ToListAsync();
            if (status.HasValue)
            {
                docs = docs.Where(d => 
                        d.Accesses.Any(a => a.DocumentStatus == status.Value))
                    .ToList();
            }

            return docs;
        }

        public async Task<bool> GrantAccessToLawyer(DocumentAccessDto documentAccessDto)
        {
            var doc = await GetDocumentWithAccesses(documentAccessDto.DocumentId);
            var lawyer = await _uow.LawyerRepo.AsQueryable().Include(l => l.User).FirstOrDefaultAsync(l => l.Id == documentAccessDto.LawyerId);

            var access = doc.Accesses.FirstOrDefault(a => a.DocumentId == doc.Id && a.LawyerId == lawyer.Id);

            if (access != null)
                throw new BadArgumentException("Lawyer already has access to this document");

            doc.Accesses.Add(new DocumentAccess { DocumentId = doc.Id, LawyerId = lawyer.Id });

            _uow.DocumentRepo.Update(doc);

            var result = await _uow.SaveChangesAsync();
            if (result <= 0)
                throw new Exception();

            await emailSender.SendEmailAsync(lawyer.User.Email, lawyer.User.Name, "اختصاص وکیل به پرونده", $"شما به پرونده با عنوان {doc.Title} اختصاص یافت.");
            await emailSender.SendEmailAsync(doc.User.Email, doc.User.Name, "اختصاص وکیل به پرونده", $"وکیل با نام {lawyer.User.Name} به پرونده با عنوان {doc.Title} اختصاص یافت.");
            //await TelegramService.SendToTelegram("اختصاص وکیل به پرونده", $"شما به پرونده با عنوان {doc.Title} اختصاص یافت.")
            return true;

        }

        public async Task<bool> TakeAccessFromLawyer(DocumentAccessDto documentAccessDto)
        {
            var doc = await GetDocumentWithAccesses(documentAccessDto.DocumentId);
            var lawyer = await _lawyerServices.GetLawyerById(documentAccessDto.LawyerId);

            var access = doc.Accesses.FirstOrDefault(a => a.DocumentId == doc.Id && a.LawyerId == lawyer.Id);

            if (access == null)
                throw new BadArgumentException("Lawyer does not have access to this document");

            doc.Accesses.Remove(access);

            _uow.DocumentRepo.Update(doc);

            var result = await _uow.SaveChangesAsync();
            if (result <= 0)
                throw new Exception();

            return true;
        }

        public async Task<List<LawyerDto>> GetLawyersThatHaveAccessToDocument(int documentId)
        {
            var doc = await GetDocumentWithAccesses(documentId);
            var lawyerIdList = doc.Accesses.Select(a => a.LawyerId).ToList();

            List<LawyerDto> lawyers = new();

            foreach (int lawyerId in lawyerIdList)
                lawyers.Add(await _lawyerServices.GetLawyerById(lawyerId));

            return lawyers;
        }

        public async Task<List<UserDto>> GetUsersThatLawyerHasAccessToTheirDocuments(int lawyerId)
        {
            var users = await _uow.DocumentRepo
                .AsQueryable()
                .Include(x => x.User)
                .Include(x => x.Accesses)
                .Where(x => x.Accesses.Select(a => a.LawyerId).Contains(lawyerId))
                .Select(x => _mapper.Map<UserDto>(x.User))
                .ToListAsync();

            users = users
                .DistinctBy(x => x.Id)
                .OrderByDescending(x => _premiumService.DoseUserHaveAnyActiveSubscription(x.Id).Result)
                .ToList();

            return users;
        }

        public async Task<List<LegalDocument>> GetDocumentsThatLawyerHasAccessToByUserId(LawyerDocumentAccessDto lawyerDocumentAccessDto,Status? status)
        {
            var docs = await _uow.DocumentRepo
                .AsQueryable()
                .Include(x => x.Accesses)
                .ThenInclude(a=>a.Lawyer)
                .Where(x => x.UserId == lawyerDocumentAccessDto.UserId && x.Accesses.Select(a => a.LawyerId).Contains(lawyerDocumentAccessDto.LawyerId))
                .ToListAsync();
            if (status.HasValue)
            {
                docs = docs.Where(d => 
                        d.Accesses.Any(a => a.DocumentStatus == status.Value))
                    .ToList();
            }
            return docs;
        }


        private async Task<LegalDocument> GetDocumentWithAccesses(int documentId)
        {
            var doc = await _uow.DocumentRepo
                .AsQueryable()
                .Include(x => x.Accesses)
                .ThenInclude(a=>a.Lawyer)
                .Include(x => x.User)
                .Where(x => x.Id == documentId)
                .FirstOrDefaultAsync();

            if (doc == null)
                throw new BadArgumentException("document not found");
            
            return doc;
        }

        public async Task UpdateDocumentStatus(DocumentStatusUpdateDto updateDto,int lawyerUserId)
        {
            var access = await _uow.DocumentAccessRepo.AsQueryable()
                .FirstOrDefaultAsync(a => a.Lawyer.UserId == lawyerUserId && a.DocumentId == updateDto.DocumentId);
            if (access is null)
                throw new BadArgumentException("document not found");
            access.DocumentStatus = updateDto.DocumentStatus;
            await _uow.SaveChangesAsync();
        }
    }
}
