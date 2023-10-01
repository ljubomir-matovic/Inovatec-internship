using AutoMapper;
using Inovatec.OfficeManagementTool.InterfacesBL;
using Inovatec.OfficeManagementTool.InterfacesUI;
using Inovatec.OfficeManagementTool.Models.Entity;
using Inovatec.OfficeManagementTool.Models.ViewModels;

namespace Inovatec.OfficeManagementTool.ImplementationsUI
{
    public class CommentUI : ICommentUI
    {
        private readonly ICommentBL _commentBL;
        private readonly IMapper _mapper;

        public CommentUI(ICommentBL commentBL, IMapper mapper)
        {
            _commentBL = commentBL;
            _mapper = mapper;
        }

        public async Task<ActionResultResponse<CommentViewModel?>> GetCommentById(int id)
        {
            return await _commentBL.GetCommentById(id);
        }

        public async Task<ActionResultResponse<DataPage<CommentViewModel>>> GetComments(CommentFilterRequest commentFilterRequest)
        {
            return await _commentBL.GetComments(commentFilterRequest);
        }

        public async Task<ActionResultResponse<CommentViewModel>> Insert(CommentCreateRequest commentCreateRequest)
        {
            return await _commentBL.Insert(_mapper.Map<Comment>(commentCreateRequest));
        }

        public async Task<ActionResultResponse<string>> Update(UpdateCommentRequest updatedComment)
        {
            return await _commentBL.Update(updatedComment);
        }

        public async Task<ActionResultResponse<string>> Delete(long id)
        {
            return await _commentBL.Delete(id);
        }
    }
}