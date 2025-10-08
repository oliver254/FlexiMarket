using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiMarket.SharedKernel.Interfaces;

public interface IQuery<TResponse> : IRequest<TResponse> { }
